FOR /L %i IN (1,1,254) DO ping -a -n 1 192.168.0.%i | FIND /i "Reply">> "C:\Users\robert.BODOCONSULT.000\Downloads\WmiAdmin\IpAddresses.txt"
pause