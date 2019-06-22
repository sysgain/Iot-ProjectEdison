# Safe Buildings Solution

## Admin Guide

### Contents
 
 - [1.0 Introduction](#10-introduction)
 - [2.0 Accessing the Edison Admin Portal Application](#20-accessing-the-edison-admin-portal-application)
 - [3.0 Security](#30-security)
      - [3.1 Storage Security](#31-storage-security)
        - [3.1.1 Secure Transfer Required](#311-secure-transfer-required)
        - [3.1.2 Advanced Threat Protection](#312-advanced-threat-protection)
        - [3.1.3 Shared Access Signature](#313-shared-access-signature)
        - [3.1.4 Encryption](#314-encryption)
      - [3.2 Cosmos DB Security](#32-cosmos-db-security)
 - [4.0 Monitoring Component](#40-monitoring-component)
    - [4.1 Application Insights](#41-application-insights)
    - [4.2 OMS Log Analytics](#42-oms-log-analytics)
 - [5. Hardening Components (Standard Solution Type)](#50-hardening-components(standard-solution-type))
    - [5.1 Primary region Configuration](#51-primary-region-configuration)
    - [5.2 Kubernates HA](#52-kubernates-ha)
      - [5.2.1 Re-Deploy the Region-2 ARM Temple](#521-re-deploy-the-region-2-arm-temple)
      - [5.2.2 Add Messaging endpoint in Bot](#522-add-messaging-endpoint-in-bot)
      - [5.2.3 Enable Enhanced authentication option](#523-enable-enhanced-authentication-option)
      - [5.2.4 Login to the Edison Dr Virtual Machine](#524-login-to-the-edison-dr-virtual-machine)
      - [5.2.5 Execute deploy1.sh](#525-execute-deploy1.sh)
      - [5.2.6 Execute configupdate2.sh](#526-execute-configupdate2.sh)
      - [5.2.7 Execute commonupdate3.sh](#527-execute-commonupdate3.sh)
      - [5.2.8 Execute edsionwebenvupdate4.sh](#528-execute-edsionwebenvupdate4.sh)
      - [5.2.9 Execute updateappsettings5.sh](#529-execute-updateappsettings5.sh)
      - [5.2.10 Execute imageupdate6.sh](#5210-execute-imageupdate6.sh)
      - [5.2.11 Execute clusterconnect7.sh](#5211-execute-clusterconnect7.sh)
      - [5.2.12 Execute set-kubernetes-config8.sh](#5212-execute-set-kubernetes-config8.sh)
      - [5.2.13 Execute updateyaml9.sh](#5213-execute-updateyaml9.sh)
      - [5.2.14 Execute ingress_custom10.sh](#5214-execute-ingress_custom10.sh)
      - [5.2.15 Update nginx-config-adminportal.yaml and nginx-config-api.yaml](#5215-update-nginx-config-adminportal.yaml-and-nginx-config-api.yaml)
      - [5.2.16 Manual configuration](#5216-manual-configuration)
     - [5.3 IoThub Failover](#53-iothub-failover)
     - [5.4 Cosmos DB Failover](#54-cosmos-db-failover)
 - [6. Premium Solution Type](#6-Premium Solution Type)
     - [6.1 Primary Region Configuration](#61-primary-region-configuration)
     - [6.2 Performing DR Strategies](#62-performing-dr-strategies)
     - [6.3 Dr Region Configuration](#63-dr-region-configuration)
     
     

## 1.0 Introduction

This document demonstrates how to perform administration activities for the Project Edison solution. In addition to the user document, this document explains the process for verifying data in the resources, updating SKUs, enabling security steps for the resources and performing DR activities for Standard and Premium solutions.

**Note**: This document is subjected to an assumption, that the solution is already deployed through ARM template, using the **Deployment Guide**.

## 2.0 Accessing the Edison Admin Portal Application
Refer to the **User Guide Document** for instructions on accessing the Edison admin portal application and the User Guide Document to onboard devices, generate activated responses, configuring responses primary and secondary range, download the history of the responses in a selected time span and receiving emergency messages, from Project Edison Solution.
The Project Edison Solution is enabling rapid response to nearby community events that might increase the danger level of persons in the nearby vicinity.

## 3.0 Security

### 3.1 Storage Security

#### 3.1.1 Secure Transfer Required

The **Secure transfer required** option enhances the security of your storage account by only allowing requests to the account from secure connections. For example, when you're calling REST APIs to access your storage account, the request must be made on HTTPS. "Secure transfer required" rejects requests that use HTTP. 

Secure transfer can be enabled by settings under configuration of the Storage account overview page.
 
![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a1.png)
 
![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a2.png)
 
![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a3.png)

#### 3.1.2 Advanced Threat Protection

Azure Storage Advanced Threat Protection detects anomalies in account activity and notifies you of any potential harmful attempts to access your account. This layer of protection allows you to address threats without the need of a security expert or manage security monitoring systems. 

On the storage account overview, click on advanced treat protection and click on **Enable Advanced Threat protection** to enable the feature.
 
![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a4.png)
 
![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a5.png)
 
![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a6.png)

#### 3.1.3 Shared Access Signature

A shared access signature token provides delegated access to resources in your storage account. With a SAS token, you can grant clients access to resources in your storage account, without sharing your account keys. This is the key point of using shared access signatures in your applications--a SAS token is a secure way to share your storage resources without compromising your account keys.
 
 ![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a7.png)
 
 ![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a8.png)

#### 3.1.4 Encryption

One way that the Azure storage platform protects your data is via Storage Service Encryption (SSE), which encrypts your data when writing it to storage, and decrypts your data when retrieving it. The encryption and decryption are automatic, transparent, and uses 256-bit AES encryption, one of the strongest block ciphers available. 

On the Settings blade for the storage account, click Encryption. Select the Use your own key option. You should already create a key in the key vault.

You can specify your key either as a URI, or by selecting the key from a key vault. 
 
 ![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a9.png)
 
 ![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a10.png)
 
*	Choose **Select from Key Vault** option.

*	Select the key vault containing the key you want to use. 

*	Select the key from the key vault. 

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a11.png)
 
Select specific key vault which is deployed through Edison Arm template.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a12.png)
 
Click on **Select** under the Encryption key section.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a13.png)
 
Choose existing key or click on **+Create** a new key, assign key name and click on Create.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a14.png)
 
Proceed to click on **Save**.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a15.png)
 
If you want to check the key vault key go to the key vault resource and click on **keys**.

![alt text](https://github.com/sysgain/Iot-ProjectEdison/raw/master/documents/Images/a16.png)
 
### 3.2 Cosmos DB Security

As a PaaS service, Cosmos DB is very easy to use. Because all user data stored in Cosmos DB is encrypted at rest and in transport, you don't have to take any action. Another way to put this is that encryption at rest is "on" by default.


4.	Monitoring Component
4.1.	Application Insights
1. Go to Resource group and click on application insights. 
 
Performance
2.	On Overview page, Summary details are displayed as shown in the following screenshot. 
3.	Click Performance on the left side of the page as shown below. 
 

 

4.	Click on View in logs from Dropdown list  select Response time in the following screenshot.

 
5.	After click on that Response time, it will open a new tab with some default queries & chart for the same.
 
6.	Back to performance then Click on Request from view in logs drop downlist, it will open a new tab with request and response operations.
 

 

7.	Click Chart icon it should be display default queries & chart. 
 
8.	Back to the Application insights overview, Click on Failures on the left side of the page as shown below.
 
9.	Click on View in Analytics and select any of the analytics. 
 
10.	After click on Request Count, it will open a new tab with some default queries & chart
 

 
Metric preview 
11.	Then select metric explorer from the left menu. 
12.	Here you need to select the resource from the drop-down list, select the metric what you want to give and select the aggregation as per requirement. 
13.	Here we can see the graph after specifying our need. 
 

 
Application Map 
14.	Application Map helps you spot performance bottlenecks or failure hotspots across all components distributed application. 
15.	After click on application map you can see the screen like below. 
 

 
16.	When you click on edisonapi it will open popup window in right side and click on Investigate Performance button.
 
17.	To check the logs, click on the chart of which you want to see the logs then you will get the logs of each request as shown in below figure.
 
18.	Similarly you can check all the services logs which are connected to the application map. 
Live Metrics Stream 
Click on Live Metric Stream to view the incoming requests, outgoing requests, overall health and servers of applications.
 

 

 


4.2.	OMS Log Analytics 
1.	Open Azure Portal  Resource Group  Click the OMS Workspace in resource group to view OMS Overview Section.
 

2.	Click Azure Resources on left side menu to view available Azure Resources.  
3.	Select your Subscription and Resource Group name from the dropdown list. 
 
4.	Click on Logs in the left side menu it will open log search box.
5.	Copy Resource group name and paste it in the search box and we write the Kusto query language and click Run button, it should show the Resource group Telemetry data.
 


 
  

6.	Here you can check the resource group information is displayed below the page as shown in the following figure. 
  

 
7.	Copy IoT Hub name and paste it in the search box with Kusto query language and click Run. 
8.	Here you can see the IoT hub resource Telemetry information is displayed below the page as show in the following figure

 

 
9.	Here you can see the IoT hub resource information is displayed below the page as show in the following figure.
 
 
10.	Copy Cosmos DB resource group name and paste it in the search box with Kusto query language and click Run. 
11.	Here you can see the Cosmos DB resource Telemetry information is displayed below the page as show in the following figure.
 
 







