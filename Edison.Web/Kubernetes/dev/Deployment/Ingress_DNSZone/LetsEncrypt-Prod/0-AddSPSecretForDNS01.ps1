﻿kubectl create secret generic azuredns-config --namespace=kube-system --from-literal=client-secret=<AzureSPSecret>