# Программа региональной балансировки нагрузки к базе данных консервативного типа на кластерной платформе "PerformSys"

Программа предназначена для эффективной балансировки нагрузки в СУБД 
консервативного типа c полной загрузкой процессорных ядер узлов кластерной 
платформы.

Переход на многоядерные процессоры повысил производительность систем, которые 
решают хорошо распараллеливаемые задачи. При этом в некоторых случаях 
производительность растет практически линейно с увеличением числа 
задействованных ядер. Однако до сих пор при работах с базами данных 
консервативного типа не использовался весь вычислительный потенциал 
современных многоядерных процессоров. 

Программа «PerformSys» позволяет:

* выполнять балансировку нагрузки между отдельными узлами кластера и между ядрами внутри каждого узла;
* задействовать все вычислительные ядра узлов кластера для обработки запросов;
* использовать вычислительные ядра наиболее эффективно с минимизацией простов.

Состав программы «PerformSys»:

* Server - модуль внутриузлового балансировщика нагрузки;
* Balancer - модуль межузлового балансировщика нагрузки;

Результатом работы программы является: балансировка нагрузки между отдельными 
узлами кластера и между ядрами внутри каждого узла, что приводит к существенному 
повышению производительности работы с базами данных консервативного типа.

## Лицензии и авторские права

Copyright © 2013-2018 Классен Роман. Все права защищены 
[лицензионным соглашением Apache 2.0](LICENSE.txt) и [приложением](NOTICE.txt) к нему.