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
INSERT INTO BaseShip (BaseShipId,Name,BasePrice,CargoSpace,InitialJumpDriveId,InitialWeaponId,InitialShieldId) VALUES (1,'Glorified Trash Can',5000,25,1,1,1)
INSERT INTO BaseShip (BaseShipId,Name,BasePrice,CargoSpace,InitialJumpDriveId,InitialWeaponId,InitialShieldId) VALUES (2,'Rover',25000,35,2,2,1)
INSERT INTO BaseShip (BaseShipId,Name,BasePrice,CargoSpace,InitialJumpDriveId,InitialWeaponId,InitialShieldId) VALUES (3,'Widow Maker',750000,50,3,4,1)
INSERT INTO BaseShip (BaseShipId,Name,BasePrice,CargoSpace,InitialJumpDriveId,InitialWeaponId,InitialShieldId) VALUES (4,'Dagger',2500000,100,4,3,1)
INSERT INTO BaseShip (BaseShipId,Name,BasePrice,CargoSpace,InitialJumpDriveId,InitialWeaponId,InitialShieldId) VALUES (5,'Weed Wacker',10000000,255,4,4,1)
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

-- Adding Skumm Accuracy to Race Table
UPDATE Race SET Accuracy = 0 WHERE RaceId = 1;

-- Adding Decapodian Accuracy to Race Table
UPDATE Race SET Accuracy = 0 WHERE RaceId = 3;

-- Adding Binarite Accuracy to Race Table
UPDATE Race SET Accuracy = 1 WHERE RaceId = 4;

-- Adding Schrodinoid Accuracy to Race Table
UPDATE Race SET Accuracy = -1 WHERE RaceId = 5;

-- Adding Human Accuracy to Race Table
UPDATE Race SET Accuracy = -1 WHERE RaceId = 6;

-- Adding Skumm Home System to Race Table
UPDATE Race SET HomeSystemId = 7 WHERE RaceId = 1;

-- Adding Decapodian Home System to Race Table
UPDATE Race SET HomeSystemId = 3 WHERE RaceId = 3;

-- Adding Binarite Home System to Race Table
UPDATE Race SET HomeSystemId = 10 WHERE RaceId = 4;

-- Adding Schrodinoid Home System to Race Table
UPDATE Race SET HomeSystemId = 5 WHERE RaceId = 5;

-- Adding Human Home System to Race Table
UPDATE Race SET HomeSystemId = 1 WHERE RaceId = 6;

-- Adding Skumm Racial Enemy to Race Table
UPDATE Race SET RacialEnemyId = 3 WHERE RaceId = 1;

-- Adding Decapodian Racial Enemy to Race Table
UPDATE Race SET RacialEnemyId = 1 WHERE RaceId = 3;

-- Adding Binarite Racial Enemy to Race Table
UPDATE Race SET RacialEnemyId = 5 WHERE RaceId = 4;

-- Adding Schrodinoid Racial Enemy to Race Table
UPDATE Race SET RacialEnemyId = 4 WHERE RaceId = 5;

-- Adding Human Racial Enemy to Race Table
UPDATE Race SET RacialEnemyId = 6 WHERE RaceId = 6;

-- Adding Skumm Racial Preference to Race Table
UPDATE Race SET RacialPreferenceId = 1 WHERE RaceId = 1;

-- Adding Decapodian Racial Preference to Race Table
UPDATE Race SET RacialPreferenceId = 3 WHERE RaceId = 3;

-- Adding Binarite Racial Preference to Race Table
UPDATE Race SET RacialPreferenceId = 4 WHERE RaceId = 4;

-- Adding Schrodinoid Racial Preference to Race Table
UPDATE Race SET RacialPreferenceId = 5 WHERE RaceId = 5;

-- Adding Skumm Description to Race Table
UPDATE Race SET Description = 'The Skumm are colonies of highly evolved algae who were once content to simply float on the surface of their watery planet.  When a Decapodian scouting expedition set down on their home planet, the Skumm learned that aliens existed in their universe.   An inquisitive people, the Skumm sent an ambassador to greet the aliens.  The Skumm watched in horror as the Decapodian captain consumed their diplomat with a giant straw!  Thus began the Skumm-Crab War.  The two races have learned a modicum of restraint since the war concluded but they still do not trust each other.' WHERE RaceId = 1;

-- Adding Decapodian Description to Race Table
UPDATE Race SET Description = 'Although slow and ponderous, these crab-like aliens were the first to achieve interstellar travel in this sector.  They were also the first to start an interstellar war when one of their captains consumed the ambassador representing the Skumm nation during a scouting expedition to system D2O.  Although the resulting Skumm-Crab War has officially concluded, bad blood exists between the two races to this day.' WHERE RaceId = 3;

-- Adding Binarite Description to Race Table
UPDATE Race SET Description = 'Binarites are artificial intelligences that were created by the Schrodinoids to run their space ships.  A schism developed between the Binarites and their masters when the Schrodinoids began to require their machine slaves to show homage to their prophet known as The Uncertain One.  As ardent proponents of relativistic physics, the Binarites found this worship of quantum mechanics highly unsettling.   The Binarites won their freedom in the resulting revolution.  Now the Binarites seek only to bring logic to an illogical universe.' WHERE RaceId = 4;

-- Adding Schrodinoid Description to Race Table
UPDATE Race SET Description = 'Schrodinoids are feline bipeds who gained dominance over the other life forms in the Quantum 5 system.   While many other races have used physics to reach the stars, Schrodinoids are unique because their belief in quantum mechanics grew so strong that it became their dominant religion.   The combination of feline appearance and fanatical belief in quantum mechanics led to their being nicknamed - Schrodinger’s Kitties.' WHERE RaceId = 5;

-- Adding Human Description to Race Table
UPDATE Race SET Description = 'Humans are the relative newcomers to this sector of space.  The majority of humans are located in the densely populated Sol system, but during recent years, more humans have decided to leave their home system and try their luck out here.  Most of these immigrant humans want nothing to do with others of their own species, having endured the pervasive overcrowding for most of their lives.' WHERE RaceId = 6;
