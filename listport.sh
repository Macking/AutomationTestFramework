tasklist /nh > test1.tmp
while read line; do
    if [ "$line" ]; then    
	echo $line >> test2.tmp
    fi
done < test1.tmp
cut -d" " -f1,2 < test2.tmp > test3.tmp

awk '/CSAcqDmn.exe/{print $2}' test3.tmp >> test4.tmp
awk '/CSWSSvc.exe/{print $2}' test3.tmp >> test4.tmp
awk '/CSDBSvc.exe/{print $2}' test3.tmp >> test4.tmp
awk '/javaw.exe/{print $2}' test3.tmp >> test4.tmp
awk '/CSDataMgr.exe/{print $2}' test3.tmp >> test4.tmp
awk '/DM.exe/{print $2}' test3.tmp >> test4.tmp

while read line; do
    netstat -nao | grep $line | grep "LISTENING"
done < test4.tmp

rm test*.tmp