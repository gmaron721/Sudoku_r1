# Sudoku_r1
### Решатель судоку для сайта grandgames.net
#### Для работы потребуется:

##### 1) Chromium 
Для Windows https://download-chromium.appspot.com/

Расположить по пути ./Resources/chrome-win

Для Linux https://download-chromium.appspot.com/?platform=Linux_x64&type=snapshots

##### 2) ChromeDrivers 
https://chromedriver.storage.googleapis.com/index.html?path=92.0.4515.43/

Расположить внутри Chromium по пути ./Resources/chrome-win/chromedriver.exe (Аналогично для Linux)

### Учетная запись
Находится в ../Resources/Account.json

### Запуск

dotnet Sudoku_r1.dll

### Комментарии
Первым делом программа определяет количество страниц.

Потом заносит все судоку с каждой страницы в базу.

Во время решения помечает судоку, чтобы не решать их дважды.

На решение одного судоку уходит 3-4 сек.

Рейтинги на сайте обновляются раз в несколько дней, поэтому сразу увидеть себя в топе не получится.

![grandgames2](https://user-images.githubusercontent.com/36194749/125776213-851f7919-9c01-4a5a-84f6-af68e60b7d49.png)
![grandgames net](https://user-images.githubusercontent.com/36194749/125776324-8bc22bac-530b-4251-9bae-5dffe3d4a6f1.png)
