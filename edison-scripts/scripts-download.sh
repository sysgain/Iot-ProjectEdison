#!/bin/bash
#downloading input.txt file
sudo wget -P /var/lib/waagent/custom-script/download/0 https://raw.githubusercontent.com/sysgain/Iot-ProjectEdison/dev-kub-ha/code/input.txt
#downloading deploy1.sh
sudo wget -P /var/lib/waagent/custom-script/download/0 https://raw.githubusercontent.com/sysgain/Iot-ProjectEdison/dev-kub-ha/edison-scripts/deploy1.sh
#downloading configupdate2.sh
sudo wget -P /var/lib/waagent/custom-script/download/0 https://raw.githubusercontent.com/sysgain/Iot-ProjectEdison/dev-kub-ha/edison-scripts/configupdate2.sh
#downloading commonupdate3.sh
sudo wget -P /var/lib/waagent/custom-script/download/0 https://raw.githubusercontent.com/sysgain/Iot-ProjectEdison/dev-kub-ha/edison-scripts/commonupdate3.sh
#downloading edisonwebenvupdate4.sh
sudo wget -P /var/lib/waagent/custom-script/download/0 https://raw.githubusercontent.com/sysgain/Iot-ProjectEdison/dev-kub-ha/edison-scripts/edisonwebenvupdate4.sh
#downloading updateappsettings5.sh
sudo wget -P /var/lib/waagent/custom-script/download/0 https://raw.githubusercontent.com/sysgain/Iot-ProjectEdison/dev-kub-ha/edison-scripts/updateappsettings5.sh
#downloading imagesupdate6.sh
sudo wget -P /var/lib/waagent/custom-script/download/0 https://raw.githubusercontent.com/sysgain/Iot-ProjectEdison/dev-kub-ha/edison-scripts/imagesupdate6.sh
#downloading clusterconnect7.sh
sudo wget -P /var/lib/waagent/custom-script/download/0 https://raw.githubusercontent.com/sysgain/Iot-ProjectEdison/dev-kub-ha/edison-scripts/clusterconnect7.sh
#downloading set-kubernetes-config8.sh
sudo wget -P /var/lib/waagent/custom-script/download/0 https://raw.githubusercontent.com/sysgain/Iot-ProjectEdison/dev-kub-ha/edison-scripts/set-kubernetes-config8.sh
#downloading updateyaml9.sh
sudo wget -P /var/lib/waagent/custom-script/download/0 https://raw.githubusercontent.com/sysgain/Iot-ProjectEdison/dev-kub-ha/edison-scripts/updateyaml9.sh
#downloading ingress_custom10.sh
sudo wget -P /var/lib/waagent/custom-script/download/0 https://raw.githubusercontent.com/sysgain/Iot-ProjectEdison/dev-kub-ha/edison-scripts/ingress_custom10.sh
#downloading secrets_ingresscontrollers11.sh
sudo wget -P /var/lib/waagent/custom-script/download/0 https://raw.githubusercontent.com/sysgain/Iot-ProjectEdison/dev-kub-ha/edison-scripts/secrets_ingresscontrollers11.sh

sudo apt-get update
sleep 40s
cd /var/lib/waagent/custom-script/download/0
chmod +x deploy1.sh configupdate2.sh commonupdate3.sh edisonwebenvupdate4.sh updateappsettings5.sh imagesupdate6.sh clusterconnect7.sh set-kubernetes-config8.sh updateyaml9.sh ingress_custom10.sh secrets_ingresscontrollers11.sh
#sh deploy1.sh https://github.com/litebulb/ProjectEdison.git