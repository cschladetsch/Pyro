set conv="C:\Program Files\Unity\Hub\Editor\2018.3.0f2\Editor\Data\Mono\lib\mono\2.0\pdb2mdb.exe"
set mono="C:\Program Files\Unity\Hub\Editor\2018.3.0f2\Editor\Data\Mono\bin\mono.exe"

for %%p in (*.dll) do %mono% %conv% %%p
