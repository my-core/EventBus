for /f "tokens=1,2,3 delims=/- " %%a in ("%date%") do @set D=%%c%%a%%b
for /f "tokens=1,2,3 delims=:." %%a in ("%time%") do @set T=%%a%%b%%c
set T=%T: =0%
set currenttime=%D%%T%
set imagetag=eventbus-publisher:%currenttime%
set dockerserver=127.0.0.1:8085
set image=%dockerserver%/%imagetag%
docker build --rm -t %imagetag% -f Dockerfile ..
docker tag %imagetag% %image%
docker login -u=xxx -p=xxx %dockerserver%
docker push %image%
pause