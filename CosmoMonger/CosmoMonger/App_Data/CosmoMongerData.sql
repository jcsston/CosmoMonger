/* CosmoMonger Data SQL Script 
If you need to update data, add to the end of this script.
The new SQL code will need to be written so that it could be ran over and over without errors, 
as this whole SQL script file is ran anything it is changed.
*/

IF NOT EXISTS (SELECT * FROM JumpDrive)
BEGIN
SET IDENTITY_INSERT JumpDrive ON
INSERT INTO JumpDrive (JumpDriveId,Name,Speed,Range,CargoCost,BasePrice) VALUES (1,'Rocket',5,10,0,1000)
INSERT INTO JumpDrive (JumpDriveId,Name,Speed,Range,CargoCost,BasePrice) VALUES (2,'Nuclear',5,15,2,10000)
INSERT INTO JumpDrive (JumpDriveId,Name,Speed,Range,CargoCost,BasePrice) VALUES (3,'Plasma',3,15,5,25000)
INSERT INTO JumpDrive (JumpDriveId,Name,Speed,Range,CargoCost,BasePrice) VALUES (4,'Light',1,20,10,100000)
SET IDENTITY_INSERT JumpDrive OFF
END;

IF NOT EXISTS (SELECT * FROM Shield)
BEGIN
SET IDENTITY_INSERT Shield ON
INSERT INTO Shield (ShieldId,Name,Strength,BasePrice,CargoCost) VALUES (1,'Force Field',10,100,1)
SET IDENTITY_INSERT Shield OFF
END;

IF NOT EXISTS (SELECT * FROM Weapon)
BEGIN
SET IDENTITY_INSERT Weapon ON
INSERT INTO Weapon (WeaponId,Name,Power,TurnCost,CargoCost,BasePrice) VALUES (1,'Railgun',8,2,0,1000)
INSERT INTO Weapon (WeaponId,Name,Power,TurnCost,CargoCost,BasePrice) VALUES (2,'Laser',5,1,2,10000)
INSERT INTO Weapon (WeaponId,Name,Power,TurnCost,CargoCost,BasePrice) VALUES (3,'Missile',15,2,5,250000)
INSERT INTO Weapon (WeaponId,Name,Power,TurnCost,CargoCost,BasePrice) VALUES (4,'Newtonian',15,1,10,1000000)
SET IDENTITY_INSERT Weapon OFF
END;

IF NOT EXISTS (SELECT * FROM BaseShip)
BEGIN
SET IDENTITY_INSERT BaseShip ON
INSERT INTO BaseShip (BaseShipId,Model,BasePrice,CargoSpace,InitialJumpDriveId,InitialWeaponId,InitialShieldId) VALUES (1,'Glorified Trash Can',5000,25,1,1,1)
INSERT INTO BaseShip (BaseShipId,Model,BasePrice,CargoSpace,InitialJumpDriveId,InitialWeaponId,InitialShieldId) VALUES (2,'Rover',25000,35,2,2,1)
INSERT INTO BaseShip (BaseShipId,Model,BasePrice,CargoSpace,InitialJumpDriveId,InitialWeaponId,InitialShieldId) VALUES (3,'Widow Maker',750000,50,3,4,1)
INSERT INTO BaseShip (BaseShipId,Model,BasePrice,CargoSpace,InitialJumpDriveId,InitialWeaponId,InitialShieldId) VALUES (4,'Dagger',2500000,100,4,3,1)
INSERT INTO BaseShip (BaseShipId,Model,BasePrice,CargoSpace,InitialJumpDriveId,InitialWeaponId,InitialShieldId) VALUES (5,'Weed Wacker',10000000,255,4,4,1)
SET IDENTITY_INSERT BaseShip OFF
END;

IF NOT EXISTS (SELECT * FROM Good)
BEGIN
SET IDENTITY_INSERT Good ON
INSERT INTO Good (GoodId,Name,BasePrice,Contraband) VALUES (1,'FoodCubes',40,0)
INSERT INTO Good (GoodId,Name,BasePrice,Contraband) VALUES (2,'Water',30,0)
INSERT INTO Good (GoodId,Name,BasePrice,Contraband) VALUES (3,'Machine Parts',75,0)
INSERT INTO Good (GoodId,Name,BasePrice,Contraband) VALUES (4,'DeathRays',150,1)
INSERT INTO Good (GoodId,Name,BasePrice,Contraband) VALUES (5,'Zowie',100,1)
INSERT INTO Good (GoodId,Name,BasePrice,Contraband) VALUES (6,'Fuel Rods',50,0)
INSERT INTO Good (GoodId,Name,BasePrice,Contraband) VALUES (7,'LuvBots',125,1)
INSERT INTO Good (GoodId,Name,BasePrice,Contraband) VALUES (8,'Fullerene',60,0)
INSERT INTO Good (GoodId,Name,BasePrice,Contraband) VALUES (9,'Nanites',175,1)
INSERT INTO Good (GoodId,Name,BasePrice,Contraband) VALUES (10,'Artificial Intelligences',200,0)
SET IDENTITY_INSERT Good OFF
END;

IF NOT EXISTS (SELECT * FROM Race)
BEGIN
SET IDENTITY_INSERT Race ON
INSERT INTO Race (RaceId,Name,Weapons,Shields,Engine,Description) VALUES (1,'Skumm',0,-1,1,'')
INSERT INTO Race (RaceId,Name,Weapons,Shields,Engine,Description) VALUES (3,'Decapodian',0,1,-1,'You''ve got crabs!')
INSERT INTO Race (RaceId,Name,Weapons,Shields,Engine,Description) VALUES (4,'Binarite',-1,0,0,'1010001001')
INSERT INTO Race (RaceId,Name,Weapons,Shields,Engine,Description) VALUES (5,'Schrodinoid',1,0,0,'Don''t put me in the box')
INSERT INTO Race (RaceId,Name,Weapons,Shields,Engine,Description) VALUES (6,'Human',1,1,-1,'Hasta la vista, baby.')
SET IDENTITY_INSERT Race OFF
END;

IF NOT EXISTS (SELECT * FROM System)
BEGIN
SET IDENTITY_INSERT System ON
INSERT INTO System (SystemId,Name,PositionX,PositionY,HasBank) VALUES (1,'Sol',0,0,1)
INSERT INTO System (SystemId,Name,PositionX,PositionY,HasBank) VALUES (2,'Vance''s Folly',6,3,0)
INSERT INTO System (SystemId,Name,PositionX,PositionY,HasBank) VALUES (3,'Glop',15,2,1)
INSERT INTO System (SystemId,Name,PositionX,PositionY,HasBank) VALUES (4,'Sigle',20,10,1)
INSERT INTO System (SystemId,Name,PositionX,PositionY,HasBank) VALUES (5,'Quantum 5',12,19,1)
INSERT INTO System (SystemId,Name,PositionX,PositionY,HasBank) VALUES (6,'11111010',3,23,0)
INSERT INTO System (SystemId,Name,PositionX,PositionY,HasBank) VALUES (7,'D2O',25,23,0)
INSERT INTO System (SystemId,Name,PositionX,PositionY,HasBank) VALUES (8,'Last Chance',8,12,0)
INSERT INTO System (SystemId,Name,PositionX,PositionY,HasBank) VALUES (9,'Haven',18,16,1)
INSERT INTO System (SystemId,Name,PositionX,PositionY,HasBank) VALUES (10,'1111101000',4,15,1)
SET IDENTITY_INSERT System OFF
END;

-- Update BaseShip models with update cargo amounts
UPDATE BaseShip SET CargoSpace = 75 WHERE BaseShipId = 4;
UPDATE BaseShip SET CargoSpace = 110 WHERE BaseShipId = 5;