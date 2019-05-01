set signer=C:\Program Files\Java\jdk1.8.0_181\bin\jarsigner.exe
set keystore=C:/Users/vrteam4/Documents/team4.keystore

powershell -Command "&'C:\Program Files\7-Zip\7z.exe' d -tzip room.apk META-INF/*"

powershell -Command "& '%signer%' -verbose -sigalg SHA1withRSA -digestalg SHA1 -keystore %keystore% '%~1' vrcap19"

pause