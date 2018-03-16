select * from Saves
where id = 32

select * from Devices
where SaveId = 32

select * from Pins
where DeviceId in (select Deviceid from Devices
where SaveId = 32)

select * from Actions
where PinId in (select PinId from Pins
where DeviceId in (select Deviceid from Devices
where SaveId = 32))
--where DeviceId = 29