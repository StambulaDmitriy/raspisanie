--определить id_login по логину
select id_login from [T_LOGIN] where [login] = 'llm' -- 2

--определить id программы по названию----------------------------------------------
select id_program from [T_PROGRAM] where [exe_name] = 'Rasp.exe' -- 25

--определить код роли по id_login и id_program
select id_role from T_MANAGE_ROLES where id_login = 2 and id_program = 25 -- ничего => нет доступа к приложению


select TOP(1) r.id_role from [T_ROLES] r
inner join 
(select [id_role] from [T_MANAGE_ROLES] where 
	[id_login] =  
				(
					select [id_login] from [T_LOGIN] where [login] = 'llm'
				)
	and 
	[id_program] = 
				(
					select [id_program] from [T_PROGRAM] where [exe_name] like 'RASP_YEAR%'
				)
) as a on r.id_role = a.id_role
order by r.role_priority DESC

------------------------------------------------------------------------------------------

select * from T_PERMIT p
inner join T_ROLE_PERMIT as rp on p.id_permit=rp.id_permit 
where id_role = 1 and id_program = 3


select * from t_roles r
inner join T_ROLES_PROGRAMS as rp on r.id_role = rp.id_role
inner join T_PROGRAM as p on rp.id_program = p.id_program
inner join T_MANAGE_ROLES as mr on mr.id_role = r.id_role
inner join T_LOGIN as l on l.id_login = mr.id_login -- 
where (l.[login] = 'llm' and p.exe_name = 'RASP_YEAR.exe')
order by r.role_priority DESC



select TOP(1) mr.id_role, mr.kod_ft, p.id_program from t_login l
inner join T_MANAGE_ROLES as mr on mr.id_login = l.id_login 
inner join T_ROLES_PROGRAMS as rp on rp.id_role = mr.id_role
inner join T_PROGRAM as p on p.id_program = rp.id_program
inner join T_ROLES as r on r.id_role = mr.id_role
where (l.[login]  = 'llm' and p.exe_name = 'RASP_YEAR.exe')
order by r.role_priority, mr.id

select p.id_permit, p.permit_name from T_PERMIT p
inner join T_ROLE_PERMIT as rp on rp.id_permit = p.id_permit
where (p.id_program = 86 and rp.id_role = 31)
order by p.id_permit


select k_ft from T_FAC_DB
where fak_name like @FakName

--регистрации расписания---------------

--выборка факультетов
select k_ft, fak_name from [T_FAC_DB]
where actual = 1
order by k_ft

--выборка видов подготовки
select distinct svp.k_vid_podg, svp.[name] from Gruppa g
inner join [sp_vid_podg] as svp on g.VID_PODG = svp.k_vid_podg
where g.actual = 1 and svp.ACTUAL = 1
--доп параметры g.K_fak

--выбор специальностей
select cod_sp, [name] from Special 
where aktual = 1
--доп параметры COD_FAK, Vid_porg

--выбор годов набора
select distinct GOD_NABORA from Gruppa
where actual = 1
--доп параметры

--выбор групп
select KOD_GRUP, NAME_GRUP from gruppa
where actual = 1
--доп параметры k_fak, vid_podg, god_nabora, kod_spec

--поиск npp_g
select * from Archiv.dbo.uch_gr
where k_fak = @k_fak and god_nabora = @god_nabora and aktual = 1 and vid_podg = @vid_podg and spec = @spec
------------------------------------------------------------------------------------------------------------



--для работы с расписанием----------------------------
--для выбора дисциплин

 SELECT * 
  FROM [ARCHIV].[dbo].[uch_plan] up
  inner join DONNTU.dbo.DISCIP as d on d.cod_disc = up.k_discip
  where up.npp_g = 2611 
  and up.k_fak = 30 
  and up.n_sem = 7 
  and d.actual = 1
  and (up.lekcii <> 0 or up.prakt <> 0 or up.laborat <> 0 or up.k_proekt <> 0 or up.k_rab <> 0) 
  and up.not_kont = 0
  order by  d.snam_rus

--для выбора дисциплин приджоенить след?
--select * from DONNTU.dbo.SP_PRIZN_DISC

--для выбора дисциплин?
--select d.cod_disc, d.NAME_RUS from DISCIP d
  --inner join ARCHIV.dbo.uch_plan as up on d.COD_DISC = up.K_DISCIP
  --where up.NPP_G = 2611 and d.ACTUAL = 1 and up.n_sem = 8 and up.k_fak = 30 and up.not_kont = 0

  ----------------------------------------------
  --для выбора видов занятий
  select id, name_nagruzka
  from [ARCHIV].[dbo].[NAGRUZKA_VID] 
  where (id between 1 and 3)
  or (id between 6 and 7)

  --для выбора аудитории по айди
  select id_aud, aud_nomer from VW_AUD_UCH
	where ID_TIP_NAZNACHENYA = 1 and aud_korpus = 7
	order by id_aud
	select * from VW_AUD_UCH
	where id_aud = 1043 and AUD_NOMER = 111

	--поиск повторяющихся аудиторий
	select id_aud, aud_korpus, count(aud_nomer) as cnt
	from [VW_AUD_UCH]
	group by id_aud,aud_korpus,aud_nomer
	having count(aud_nomer) > 1

	-- для выбора преподавателей 
	 select distinct tabn_s, fio 
	 from [VW_PersonalPPSALL] 

 
--для добавления пар в расписание 

--сохранение регистрации
	insert into [UUS].[dbo].[RASP_YEAR] (KOD_GRUP,UCH_GOD,N_SEM,KOD_FO,VID_PODG,KURS,NPP_G,K_FT)
	values 
	(@kod_grup, @uch_god, @n_sem, 
		(select k_fo from [ARCHIV].[dbo].[uch_gr] ug
		inner join [DONNTU].[dbo].[Gruppa] as g on ug.npp_g = g.npp_g_up and ug.k_fak = g.k_fak
		where g.KOD_GRUP = @kod_grup),
		@vid_podg, @kurs, @npp_g, @k_ft)

--сохранение пар расписания
  insert UUS.dbo.[RASP_DISCIP] (ID_RASP_YEAR,D_NED,PR_NED,ID_RASP_TIME,ID_NAGRUZKA_VID,ID_AUD,K_DISCIP,DATE_S,DATE_PO,ACTUAL)
  values(@id_rasp_year, 
  (select id from [UUS].[dbo].[SP_DAY]
	where [name] like @d_ned),
	@pr_ned,
	( select * from [UUS].[dbo].[VW_RASP_Time]
		where nom = @i and actual = 1)
	,@vid_podg,@codeAudit,@codeDisc,GETDATE(),NULL)

-- сохранение препода в RASP_PPS
 insert into [UUS].[dbo].[RASP_PPS]
  (ID_RASP_DISCIP,TABN_S,DATE_S)
  values
  (
	(
		select top(1) id from [UUS].[dbo].[RASP_DISCIP] order by id DESC
	),@tabn_s,GETDATE()
  )

  --поиск существующего расписания
  select * from [UUS].[dbo].[RASP_YEAR]
  where KOD_GRUP = 8908 and UCH_GOD = 2018 and N_SEM = @n_sem and k_ft = 30

  --считывание пар 
  select  distinct * from [UUS].[dbo].[RASP_DISCIP] rd
  inner join [UUS].[dbo].[SP_DAY] as sd on sd.id = rd.D_NED
  inner join [UUS].[dbo].[VW_RASP_Time] as vrt on vrt.ID = rd.ID_RASP_TIME
  inner join [UUS].[dbo].[RASP_PPS] as rp on rp.ID_RASP_DISCIP = rd.ID
  where rd.ID_RASP_YEAR = 15 and rd.ACTUAL = 1
  order by sd.OrderName

  --
  SELECT up.k_discip, d.snam_rus
  FROM [ARCHIV].[dbo].[uch_plan] up
  inner join DONNTU.dbo.DISCIP as d on d.cod_disc = up.k_discip
  where up.k_discip = 15
  
  select snam_rus from DONNTU.dbo.DISCIP
  where cod_disc = 15
  
  select name_nagruzka
  from[ARCHIV].[dbo].[NAGRUZKA_VID]
  where id = 7

  select AUD_NOMER_S from [DONNTU].[dbo].[VW_AUD_UCH]
  where ID_AUD = 939

  select distinct fio
  from [DONNTU].[dbo].[VW_PersonalPPSALL]
  where TABN_S = 6599

  -- выбор расписания для просмотра/редактирования
  select 
  ry.id as id, ry.KOD_GRUP as kod_grup, ry.UCH_GOD as uch_god, ry.N_SEM as n_sem, KOD_FO as kod_fo,
  ry.vid_podg as vid_podg, ry.KURS as kurs, ry.NPP_G as npp_g, ry.K_FT as k_ft, g.KOD_SPEC as kod_spec,
  g.GOD_NABORA as god_nabora, g.NAME_GRUP_RUS as name_grup, tfd.fak_name as fak_name, svp.[name] as namepodg 
  from [UUS].[dbo].[RASP_YEAR] ry
  inner join [DONNTU].[dbo].[Gruppa] as g on ry.KOD_GRUP = g.KOD_GRUP
  inner join [DONNTU].[dbo].[SPECIAL] as s on s.COD_SP = g.KOD_SPEC
  inner join [ARCHIV].[dbo].[T_FAC_DB] as tfd on ry.k_ft = tfd.k_ft 
  inner join [DONNTU].[dbo].[sp_vid_podg] as svp on svp.k_vid_podg = ry.VID_PODG