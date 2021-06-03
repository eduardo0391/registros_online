using NHibernate;
using NHibernate.Cfg;
using NHibernate.Criterion;
using System;
using System.Collections;
using System.Collections.Generic;

using System.Net.Mail;
using System.Web.Configuration;
using NHibernate.Transform;
using System.Web.Hosting;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace registros_online.Models
{

    public class dbContext
    {
        //Define the session factory, this is per database 
        ISessionFactory sessionFactory;
        IList<User> usuarios;
        public byte[] Clave = Encoding.ASCII.GetBytes("aa22");
        public byte[] IV = Encoding.ASCII.GetBytes("Devjoker7.37hAES");
        /// <summary>
        /// Method to create session and manage entities
        /// </summary>
        /// <returns></returns>
        public string directorio { get; set; }

        public dbContext()
        {
            directorio = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath.ToString();
        }

        ISession OpenSession()
        {
            if (sessionFactory == null)
            {
                var cgf = new Configuration();
                var data = cgf.Configure(directorio + "\\hibernate.cfg.xml");
                cgf.AddDirectory(new System.IO.DirectoryInfo(directorio + "\\bin\\NHibernate\\Mapping"));
                sessionFactory = data.BuildSessionFactory();
            }

            return sessionFactory.OpenSession();
        }

        public IList<User> getUsuarios()
        {
            using (ISession session = OpenSession())
            {
                IQuery query;
                query = session.CreateQuery("from User");
                usuarios = query.List<User>();
            }
            return usuarios;
        }

        public void crearUsuario(User user)
        {
            user.password = Encriptar(user.password);
            using (ISession session = OpenSession())
            {
                //Perform transaction
                using (ITransaction tran = session.BeginTransaction())
                {
                    session.Save(user);
                    tran.Commit();
                }
            }

        }


        public dataModel GetRegisters(string anio, string mes, string columna, int id_usuario, string tipo, string orden = "fecha", string pagina = "10", int numeroPagina = 2)
        {
            dataModel datosDevuelto = new dataModel();
            using (ISession session = OpenSession())
            {
                ICriteria crit = session.CreateCriteria<Register>();
                crit.Add(Expression.Sql("year(fecha)= ?", anio, NHibernateUtil.String));
                crit.Add(Expression.Sql("month(fecha)= ?", mes, NHibernateUtil.String));
                crit.Add(Expression.Sql("id_user= ?", id_usuario, NHibernateUtil.Int32));
                crit.Add(Expression.Sql("tipo= ?", tipo, NHibernateUtil.String));
                crit.AddOrder(Order.Asc(orden));
                datosDevuelto.cantidad = crit.List().Count;
                if (pagina != "todo") {
                    crit.SetFirstResult((numeroPagina - 1) * int.Parse(pagina));
                    crit.SetMaxResults(int.Parse(pagina));
                }
                datosDevuelto.registros = crit.List<Register>();
                
            }
            return datosDevuelto;
        }

        public dataModel GetTodo( int id_usuario, string tipo, string orden = "fecha")
        {
            dataModel datosDevuelto = new dataModel();
            using (ISession session = OpenSession())
            {
                ICriteria crit = session.CreateCriteria<Register>();
   
                crit.Add(Expression.Sql("id_user= ?", id_usuario, NHibernateUtil.Int32));
                crit.Add(Expression.Sql("tipo= ?", tipo, NHibernateUtil.String));
                crit.AddOrder(Order.Asc(orden));
                datosDevuelto.cantidad = crit.List().Count;
                datosDevuelto.registros = crit.List<Register>();

            }
            return datosDevuelto;
        }

        public IList GetRegistrosxMes(int id_usuario, string tipo)
        {
            IList lista = null;
            using (ISession session = OpenSession())
            {
                string valor = "select year(fecha) as anio, month(fecha) as mes, ROUND(sum(precioUnitario),2) as precio from movimientos  where tipo ='" + tipo + "' and id_user = '" + id_usuario
                 + "' group by year(fecha), MONTH(fecha) order by YEAR(fecha) desc, MONTH(fecha) desc";
                lista = session.CreateSQLQuery(valor).List();
            }
            return lista;

        }

        public string getUltimoAnio(string tipo, int id_usuario)
        {
            IList resultado;
            using (ISession session = OpenSession())
            {
                string sql = "select year(max(fecha)) from movimientos where id_user = '" + id_usuario + "'  and  tipo =" + tipo;
                resultado = session.CreateSQLQuery(sql).List();
            }
            if (resultado[0] == null)
                return DateTime.Now.Year.ToString();
            else
            {
                return resultado[0].ToString();
            }

        }

        internal IList<Category> getCategorias(int tipo, int id_usuario)
        {

            IList<Category> categorias = new List<Category>();
            // IList lista = null;
            using (ISession session = OpenSession())
            {
                categorias = session.CreateQuery("From Category where id_tipo = " + tipo + " and id_user = " + id_usuario + " order by descripcion").List<Category>();
            }
            return categorias;

        }


          public MovementStatisticsViewModel getEstadisticaProducto(string fechas, int id_usuario, string tipo)
        {
            /*            IList lista = null*/
            MovementStatisticsViewModel listStatistics = new MovementStatisticsViewModel();
            IList<MovementStatistics> listaTop10 = new List<MovementStatistics>();
            IList<MovementStatistics> listaFull = new List<MovementStatistics>();
            using (ISession session = OpenSession())
            {
                //top 10
                string sql = "select top 10 movimiento, sum(total) as total, fechaFormateada from estadisticasMovimientos where id_user ='" + id_usuario + "' and  tipo ='" + tipo + "' and  fechaFormateada in (" + fechas + ") group by movimiento,fechaFormateada";
                var aux = session.CreateSQLQuery(sql).List();
                foreach (var x in aux)
                {
                    listaTop10.Add(new MovementStatistics()
                    {
                        movimiento = ((object[])x)[0].ToString(),
                        total = float.Parse(((object[])x)[1].ToString()),
                        fechaFormateada = ((object[])x)[2].ToString()
                    });
                }
                listStatistics.top10 = listaTop10;

                //full
                sql = "select movimiento, sum(total) as total,fechaFormateada from estadisticasMovimientos where id_user ='" + id_usuario + "' and  tipo ='" + tipo + "' and  fechaFormateada in (" + fechas + ") group by movimiento,fechaFormateada order by total desc";
                aux = session.CreateSQLQuery(sql).List();
                foreach (var x in aux)
                {
                    listaFull.Add(new MovementStatistics()
                    {
                        movimiento = ((object[])x)[0].ToString(),
                        total = float.Parse(((object[])x)[1].ToString()),
                        fechaFormateada = ((object[])x)[2].ToString()
                    });
                }
                listStatistics.full = listaFull;

            }
            return listStatistics;
        }

        public IList<int> getAños(int id_usuario, string tipo)
        {
            IList<int> listaAño;
            using (ISession session = OpenSession())
            {
                //NHibernate query
                IQuery query = session.CreateSQLQuery("select year(fecha) as año from movimientos where tipo =" + tipo + " and id_user='" + id_usuario + "' group by year(fecha) order by año ");
                listaAño = query.List<int>();

            }
            return listaAño;
        }

        public IList<int> getMes(string año, int id_usuario, string tipo)
        {
            IList<int> listaMes;
            using (ISession session = OpenSession())
            {
                IQuery query;
                if (año == null || año == "undefined")
                {
                    query = session.CreateSQLQuery("select MONTH(fecha) from movimientos where tipo = " + tipo + " and id_user='" + id_usuario + "' group by month(fecha);");
                }
                else
                {
                    query = session.CreateSQLQuery("select MONTH(fecha) from movimientos where year(fecha) = " + año.ToString() + " and tipo = " + tipo + " and id_user='" + id_usuario + "' group by month(fecha);");
                }
                listaMes = query.List<int>();

            }
            return listaMes;
        }

        public UsuarioEmailViewModel getUsuarioByUserMail(string usuario, string email)
        {
            UsuarioEmailViewModel user = null;
            using (ISession session= OpenSession())
            {
                IQuery query;
                query = session.CreateSQLQuery("select(select count(*) from usuarios where usuario ='"+usuario+"' ) usuario, (select count(*) from usuarios where email = '"+email+"') email");
                user = query.SetResultTransformer(Transformers.AliasToBean<UsuarioEmailViewModel>()).UniqueResult<UsuarioEmailViewModel>();
                
            }
            return user;
        }

        public UsuarioEmailViewModel getUsuarioActualizar(string usuario, string email, int id)
        {
            UsuarioEmailViewModel user = null;
            using (ISession session = OpenSession())
            {
                IQuery query;
                query = session.CreateSQLQuery("select(select count(*) from usuarios where id_usuario <> " + id+" and usuario ='" + usuario 
                + "' ) usuario, (select count(*) from usuarios where id_usuario <> " + id + " and email = '" + email + "') email");
                user = query.SetResultTransformer(Transformers.AliasToBean<UsuarioEmailViewModel>()).UniqueResult<UsuarioEmailViewModel>();

            }
            return user;
        }

        public User getUsuarioByUserMail(LoginViewModel varUser)
        {
            User user = new User();
            using (ISession session = OpenSession())
            {
                user = session.QueryOver<User>().Where(x => x.user == varUser.user || x.email == varUser.user).SingleOrDefault();
            }
            return user;
        }

        public User getUsuarioById(int id)
        {
            User user = new User();
            using (ISession session = OpenSession())
            {
                user = session.Get<User>(id);
            }
            return user;
        }

        public User getUsuarioByEmail(string email)
        {
            User user = new User();
            using (ISession session = OpenSession())
            {
                user = session.QueryOver<User>().Where(x => x.email == email).SingleOrDefault();
            }
            return user;
        }

        public User getUsuarioxCodigo(string codigo)
        {
            User user;

            using (ISession session = OpenSession())
            {
                 user = session.QueryOver<User>().Where(x => x.codActivation == codigo).SingleOrDefault();
             }
            return  user;
        }

        public User getUsuario(string varUser)
        {
            User user = new User();
            using (ISession session = OpenSession())
            {
                user = session.QueryOver<User>().Where(x => x.user == varUser).SingleOrDefault();
            }
            return user;
        }

        public Register GetRegistroById(int Id)
        {
            Register reg = new Register();
            using (ISession session = OpenSession())
            {
                reg = session.Get<Register>(Id);
            }
            return reg;
        }

        public Category getCategoria(int Id)
        {
            Category reg = new Category();
            using (ISession session = OpenSession())
            {
                reg = session.Get<Category>(Id);
            }
            return reg;
        }

        public int CreateRegistro(Register reg)
        {
         
            int regNro = 0;

            using (ISession session = OpenSession())
            {
                //Perform transaction
                using (ITransaction tran = session.BeginTransaction())
                {
                    session.Save(reg);
                    tran.Commit();
                }
            }

            return regNro;
        }

        public int createUsuario(Register reg)
        {
            int regNro = 0;

            using (ISession session = OpenSession())
            {
                //Perform transaction
                using (ITransaction tran = session.BeginTransaction())
                {
                    session.Save(reg);
                    tran.Commit();
                }
            }

            return regNro;
        }

        public void UpdatePrueba(Register reg)
        {
            using (ISession session = OpenSession())
            {
                using (ITransaction tran = session.BeginTransaction())
                {
                    session.Update(reg);
                    tran.Commit();
                }
            }
        }

        public void UpdateUsuario(User usuario)
        {
            using (ISession session = OpenSession())
            {
                using (ITransaction tran = session.BeginTransaction())
                {
                    session.Update(usuario);
                    tran.Commit();
                }
            }
        }

        public void DeletePrueba(Register reg)
        {
            using (ISession session = OpenSession())
            {
                using (ITransaction tran = session.BeginTransaction())
                {
                    session.Delete(reg);
                    tran.Commit();
                }
            }
        }
  
        public void SendAsync(string emailDestino, string activationCode)
        {
           var url = WebConfigurationManager.AppSettings["direccionPagina"];
            string  subject = "Recuperacion de contraseña - Sistema de registro Online";
            string body = "Tu solicitud para recuperar tu contraseña se ha realizado con exito <br/> <br/>" +
            "Haga click en el siguiente link para obtener una nueva contraseña <br/><br/> <a href=http://" + url + "/usuario/verificarCuenta?codigo="+ activationCode + "> recuperar clave</a>";
            enviarMail(emailDestino, subject, body);
        }

        public void actualizarCategoria(Category categoria)
        {
            using (ISession session = OpenSession())
            {
                //Perform transaction
                using (ITransaction tran = session.BeginTransaction())
                {
                    session.Update(categoria);
                    tran.Commit();
                }
            }
        }

        public void agregarCategoria(Category categoria)
        {
            
            using (ISession session = OpenSession())
            {
                //Perform transaction
                using (ITransaction tran = session.BeginTransaction())
                {
                    session.Save(categoria);
                    tran.Commit();
                }
            }
          //  return categoria;
        }
      
        public void eliminarCategoria(Category cate)
        {
            using (ISession session = OpenSession())
            {
                using (ITransaction tran = session.BeginTransaction())
                {
                    session.Delete(cate);
                    tran.Commit();
                }
            }
        }

        public void enviarMailConfirmacion(int? id, string emailDestino)
        {
            try
            {
                string url = WebConfigurationManager.AppSettings["direccionPagina"];
                string cuerpo = System.IO.File.ReadAllText(HostingEnvironment.MapPath("~/templateMail/mensajeCorreo.cshtml"));
                string subject = "Confirme su cuenta en Mis registros online";
                string urlPagina = url + "/usuario/confirmarEmail?id=" + id;
                cuerpo = cuerpo.Replace("urlBoton", urlPagina);
                enviarMail(emailDestino, subject, cuerpo);
            }
            catch (Exception ex)
            {
                throw (ex);
            }

            }

        public void enviarMail(string emailDestino, string subject, string body)
        {
            try
            {
                MailMessage m = new MailMessage();
                SmtpClient sc = new SmtpClient();
                m.From = new MailAddress("misregistrosonline123@gmail.com");
                m.To.Add(emailDestino);
                m.Subject = subject; ;
                m.IsBodyHtml = true;
                m.Body = body;;
                sc.Host = "smtp.gmail.com";
                sc.Port = 587;
                sc.Credentials = new System.Net.NetworkCredential("misregistrosonline123@gmail.com", "bolpesa2");

                sc.EnableSsl = true;
                sc.Send(m);


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void cargarCategorias(int? usuario)
        {

            using (ISession session = OpenSession())
            {
                IQuery query = session.CreateSQLQuery("exec agregarCategorias :usuario");
                query.SetParameter("usuario", usuario);
     
                object obj = query.UniqueResult();

            }
        }

        public Category chequearCategoria(Category cate)
        {
            Category aux = null;
            using (ISession session = OpenSession())
            {
                ICriteria crit = session.CreateCriteria<Category>();

                crit.Add(Expression.Sql("id_tipo= ?", cate.id_tipo, NHibernateUtil.Int32));
                crit.Add(Expression.Sql("id_user= ?", cate.id_user, NHibernateUtil.Int32));
                crit.Add(Expression.Sql("descripcion= ?", cate.descripcion, NHibernateUtil.String));
                crit.AddOrder(Order.Asc("descripcion"));
                if (crit.List<Category>().Count>0)
                aux = crit.List<Category>()[0];

            }
            return aux;
        }


        /// Encripta una cadena
        public string Encriptar( string _cadenaAencriptar)
        {
            string result = string.Empty;
            byte[] encryted = System.Text.Encoding.Unicode.GetBytes(_cadenaAencriptar);
            result = Convert.ToBase64String(encryted);
            return result;
        }

        /// Esta función desencripta la cadena que le envíamos en el parámentro de entrada.
        public string DesEncriptar (string _cadenaAdesencriptar)
        {
            string result = string.Empty;
            byte[] decryted = Convert.FromBase64String(_cadenaAdesencriptar);
            //result = System.Text.Encoding.Unicode.GetString(decryted, 0, decryted.ToArray().Length);
            result = System.Text.Encoding.Unicode.GetString(decryted);
            return result;
        }

        public UserPermission GetPermission(int id_usuario)
        {
            UserPermission UserPermission = new UserPermission();
            if (id_usuario != 0)
            using (ISession session = OpenSession())
            {
                    UserPermission = session.Get<UserPermission>(id_usuario);
            }
            return UserPermission;
        }

    }
}
