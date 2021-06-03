
 CREATE view [dbo].[UserPermissions]
 as
      select 
	permission.id_usuario, 
	permission.id_permission,
	ed.Description, 
	case when permission.id_permission in (2,3,5) then 1
	else  0
	end as IsRequiredPay
	from			
				(select id_usuario, case 
				when IsSuperUser =1 then 1
				when (confirmado != 1)  then 4
                when DATEDIFF(DAY, fechaEnvio, GETDATE()) > 31 and expirationDate is null then 2
                when(select count(*) from movimientos where id_user = id_usuario) > 30 and expirationDate is null  then 3
				when expirationDate < GETDATE() then 5
				
	              else 1
                end as id_permission
	       from usuarios) permission
	 left join errorDescription ed 
	 on permission.id_permission = ed.Id   

