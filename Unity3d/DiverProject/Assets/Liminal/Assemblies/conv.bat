rem for %%v in (*.dll) do "C:\Program Files\Unity\Editor\Data\MonoBleedingEdge\bin\mono" "C:\Program Files\Unity\Editor\Data\MonoBleedingEdge\lib\mono\4.5\pdb2mdb.exe" %%v

set edge="C:\Program Files\Unity\Hub\Editor\2018.3.0f2\Editor\Data\MonoBleedingEdge
set mono=%edge%\mono.exe"
set conv=%edge%\lib\mono\4.5\pdb2mdp.exe"

for %%v in (*.dll) do %mono% %conv% %%v

