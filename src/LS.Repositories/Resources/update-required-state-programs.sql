UPDATE LP SET RequiredStateProgramID=SP.Id 
FROM LifelinePrograms LP 
LEFT JOIN StatePrograms SP ON SP.Name='DCN' 
WHERE LP.StateCode='MO' 
     AND LP.ProgramName NOT IN ('Federal Housing Assistance (Section 8)', 'National School Lunch (free program only)', 'Supplemental Security Income', 'Annual Income') 
 
UPDATE LP SET RequiredStateProgramID=SP.Id 
FROM LifelinePrograms LP 
LEFT JOIN StatePrograms SP ON SP.Name='DSHS' 
WHERE LP.StateCode='WA' 
     AND LP.ProgramName IN ('Food Stamps','Supplemental Security Income','Temporary Assistance for Needy Families','Medicaid') 
 
UPDATE LP SET RequiredStateProgramID=SP.Id 
FROM LifelinePrograms LP 
LEFT JOIN StatePrograms SP ON SP.Name='Control' 
WHERE LP.StateCode='PR' 
 
UPDATE LP SET RequiredSecondaryStateProgramID=SP.Id 
FROM LifelinePrograms LP 
LEFT JOIN StatePrograms SP ON SP.Name='Certificate' 
WHERE LP.StateCode='PR'