select m.Name, c.Id as chunkId, l.Id as locationId, * from ChunkInfo c
inner join LocationInfo l on c.LocationInfoId = l.Id
inner join MetaInfo	m on m.Id = c.MetaInfoId
where m.Name not like '(%'
order by chunkId

select * from ChunkInfo