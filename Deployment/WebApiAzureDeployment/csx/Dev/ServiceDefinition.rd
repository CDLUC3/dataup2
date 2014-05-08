<?xml version="1.0" encoding="utf-8"?>
<serviceModel xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" name="WebApiAzureDeployment" generation="1" functional="0" release="0" Id="62a78096-7299-4c73-8d3b-a32d8dc36bfe" dslVersion="1.2.0.0" xmlns="http://schemas.microsoft.com/dsltools/RDSM">
  <groups>
    <group name="WebApiAzureDeploymentGroup" generation="1" functional="0" release="0">
      <componentports>
        <inPort name="WebApi:dataup2httpsin" protocol="http">
          <inToChannel>
            <lBChannelMoniker name="/WebApiAzureDeployment/WebApiAzureDeploymentGroup/LB:WebApi:dataup2httpsin" />
          </inToChannel>
        </inPort>
      </componentports>
      <settings>
        <aCS name="FilePurgeService:DataOnBoardingStorage" defaultValue="">
          <maps>
            <mapMoniker name="/WebApiAzureDeployment/WebApiAzureDeploymentGroup/MapFilePurgeService:DataOnBoardingStorage" />
          </maps>
        </aCS>
        <aCS name="FilePurgeService:Microsoft.Research.DataOnboarding" defaultValue="">
          <maps>
            <mapMoniker name="/WebApiAzureDeployment/WebApiAzureDeploymentGroup/MapFilePurgeService:Microsoft.Research.DataOnboarding" />
          </maps>
        </aCS>
        <aCS name="FilePurgeService:Microsoft.Research.DataOnboarding.FilePurgeService" defaultValue="">
          <maps>
            <mapMoniker name="/WebApiAzureDeployment/WebApiAzureDeploymentGroup/MapFilePurgeService:Microsoft.Research.DataOnboarding.FilePurgeService" />
          </maps>
        </aCS>
        <aCS name="FilePurgeService:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="">
          <maps>
            <mapMoniker name="/WebApiAzureDeployment/WebApiAzureDeploymentGroup/MapFilePurgeService:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </maps>
        </aCS>
        <aCS name="FilePurgeService:PrimaryContainer" defaultValue="">
          <maps>
            <mapMoniker name="/WebApiAzureDeployment/WebApiAzureDeploymentGroup/MapFilePurgeService:PrimaryContainer" />
          </maps>
        </aCS>
        <aCS name="FilePurgeService:ScheduledTimeInHours" defaultValue="">
          <maps>
            <mapMoniker name="/WebApiAzureDeployment/WebApiAzureDeploymentGroup/MapFilePurgeService:ScheduledTimeInHours" />
          </maps>
        </aCS>
        <aCS name="FilePurgeService:UploadedFilesExpirationDurationInHours" defaultValue="">
          <maps>
            <mapMoniker name="/WebApiAzureDeployment/WebApiAzureDeploymentGroup/MapFilePurgeService:UploadedFilesExpirationDurationInHours" />
          </maps>
        </aCS>
        <aCS name="FilePurgeServiceInstances" defaultValue="[1,1,1]">
          <maps>
            <mapMoniker name="/WebApiAzureDeployment/WebApiAzureDeploymentGroup/MapFilePurgeServiceInstances" />
          </maps>
        </aCS>
        <aCS name="PublishProcessor:DataOnBoardingStorage" defaultValue="">
          <maps>
            <mapMoniker name="/WebApiAzureDeployment/WebApiAzureDeploymentGroup/MapPublishProcessor:DataOnBoardingStorage" />
          </maps>
        </aCS>
        <aCS name="PublishProcessor:Microsoft.Research.DataOnboarding" defaultValue="">
          <maps>
            <mapMoniker name="/WebApiAzureDeployment/WebApiAzureDeploymentGroup/MapPublishProcessor:Microsoft.Research.DataOnboarding" />
          </maps>
        </aCS>
        <aCS name="PublishProcessor:Microsoft.Research.DataOnboarding.QueueService" defaultValue="">
          <maps>
            <mapMoniker name="/WebApiAzureDeployment/WebApiAzureDeploymentGroup/MapPublishProcessor:Microsoft.Research.DataOnboarding.QueueService" />
          </maps>
        </aCS>
        <aCS name="PublishProcessor:Microsoft.Research.DataOnboarding.Services.PublishProcessor" defaultValue="">
          <maps>
            <mapMoniker name="/WebApiAzureDeployment/WebApiAzureDeploymentGroup/MapPublishProcessor:Microsoft.Research.DataOnboarding.Services.PublishProcessor" />
          </maps>
        </aCS>
        <aCS name="PublishProcessor:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="">
          <maps>
            <mapMoniker name="/WebApiAzureDeployment/WebApiAzureDeploymentGroup/MapPublishProcessor:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </maps>
        </aCS>
        <aCS name="PublishProcessor:PrimaryContainer" defaultValue="">
          <maps>
            <mapMoniker name="/WebApiAzureDeployment/WebApiAzureDeploymentGroup/MapPublishProcessor:PrimaryContainer" />
          </maps>
        </aCS>
        <aCS name="PublishProcessor:ScheduledLogTransferInMinutes" defaultValue="">
          <maps>
            <mapMoniker name="/WebApiAzureDeployment/WebApiAzureDeploymentGroup/MapPublishProcessor:ScheduledLogTransferInMinutes" />
          </maps>
        </aCS>
        <aCS name="PublishProcessor:ScheduledTime" defaultValue="">
          <maps>
            <mapMoniker name="/WebApiAzureDeployment/WebApiAzureDeploymentGroup/MapPublishProcessor:ScheduledTime" />
          </maps>
        </aCS>
        <aCS name="PublishProcessorInstances" defaultValue="[1,1,1]">
          <maps>
            <mapMoniker name="/WebApiAzureDeployment/WebApiAzureDeploymentGroup/MapPublishProcessorInstances" />
          </maps>
        </aCS>
        <aCS name="WebApi:DataOnBoardingStorage" defaultValue="">
          <maps>
            <mapMoniker name="/WebApiAzureDeployment/WebApiAzureDeploymentGroup/MapWebApi:DataOnBoardingStorage" />
          </maps>
        </aCS>
        <aCS name="WebApi:Microsoft.Research.DataOnboarding" defaultValue="">
          <maps>
            <mapMoniker name="/WebApiAzureDeployment/WebApiAzureDeploymentGroup/MapWebApi:Microsoft.Research.DataOnboarding" />
          </maps>
        </aCS>
        <aCS name="WebApi:Microsoft.Research.DataOnboarding.FileService" defaultValue="">
          <maps>
            <mapMoniker name="/WebApiAzureDeployment/WebApiAzureDeploymentGroup/MapWebApi:Microsoft.Research.DataOnboarding.FileService" />
          </maps>
        </aCS>
        <aCS name="WebApi:Microsoft.Research.DataOnboarding.FileService.FileProcesser" defaultValue="">
          <maps>
            <mapMoniker name="/WebApiAzureDeployment/WebApiAzureDeploymentGroup/MapWebApi:Microsoft.Research.DataOnboarding.FileService.FileProcesser" />
          </maps>
        </aCS>
        <aCS name="WebApi:Microsoft.Research.DataOnboarding.RepositoryAdapters.SkyDrive" defaultValue="">
          <maps>
            <mapMoniker name="/WebApiAzureDeployment/WebApiAzureDeploymentGroup/MapWebApi:Microsoft.Research.DataOnboarding.RepositoryAdapters.SkyDrive" />
          </maps>
        </aCS>
        <aCS name="WebApi:Microsoft.Research.DataOnboarding.WebApi" defaultValue="">
          <maps>
            <mapMoniker name="/WebApiAzureDeployment/WebApiAzureDeploymentGroup/MapWebApi:Microsoft.Research.DataOnboarding.WebApi" />
          </maps>
        </aCS>
        <aCS name="WebApi:Microsoft.Research.DataOnboarding.WebApi.Api" defaultValue="">
          <maps>
            <mapMoniker name="/WebApiAzureDeployment/WebApiAzureDeploymentGroup/MapWebApi:Microsoft.Research.DataOnboarding.WebApi.Api" />
          </maps>
        </aCS>
        <aCS name="WebApi:Microsoft.Research.DataOnboarding.WebApi.Security" defaultValue="">
          <maps>
            <mapMoniker name="/WebApiAzureDeployment/WebApiAzureDeploymentGroup/MapWebApi:Microsoft.Research.DataOnboarding.WebApi.Security" />
          </maps>
        </aCS>
        <aCS name="WebApi:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="">
          <maps>
            <mapMoniker name="/WebApiAzureDeployment/WebApiAzureDeploymentGroup/MapWebApi:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </maps>
        </aCS>
        <aCS name="WebApi:PrimaryContainer" defaultValue="">
          <maps>
            <mapMoniker name="/WebApiAzureDeployment/WebApiAzureDeploymentGroup/MapWebApi:PrimaryContainer" />
          </maps>
        </aCS>
        <aCS name="WebApiInstances" defaultValue="[1,1,1]">
          <maps>
            <mapMoniker name="/WebApiAzureDeployment/WebApiAzureDeploymentGroup/MapWebApiInstances" />
          </maps>
        </aCS>
      </settings>
      <channels>
        <lBChannel name="LB:WebApi:dataup2httpsin">
          <toPorts>
            <inPortMoniker name="/WebApiAzureDeployment/WebApiAzureDeploymentGroup/WebApi/dataup2httpsin" />
          </toPorts>
        </lBChannel>
      </channels>
      <maps>
        <map name="MapFilePurgeService:DataOnBoardingStorage" kind="Identity">
          <setting>
            <aCSMoniker name="/WebApiAzureDeployment/WebApiAzureDeploymentGroup/FilePurgeService/DataOnBoardingStorage" />
          </setting>
        </map>
        <map name="MapFilePurgeService:Microsoft.Research.DataOnboarding" kind="Identity">
          <setting>
            <aCSMoniker name="/WebApiAzureDeployment/WebApiAzureDeploymentGroup/FilePurgeService/Microsoft.Research.DataOnboarding" />
          </setting>
        </map>
        <map name="MapFilePurgeService:Microsoft.Research.DataOnboarding.FilePurgeService" kind="Identity">
          <setting>
            <aCSMoniker name="/WebApiAzureDeployment/WebApiAzureDeploymentGroup/FilePurgeService/Microsoft.Research.DataOnboarding.FilePurgeService" />
          </setting>
        </map>
        <map name="MapFilePurgeService:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" kind="Identity">
          <setting>
            <aCSMoniker name="/WebApiAzureDeployment/WebApiAzureDeploymentGroup/FilePurgeService/Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </setting>
        </map>
        <map name="MapFilePurgeService:PrimaryContainer" kind="Identity">
          <setting>
            <aCSMoniker name="/WebApiAzureDeployment/WebApiAzureDeploymentGroup/FilePurgeService/PrimaryContainer" />
          </setting>
        </map>
        <map name="MapFilePurgeService:ScheduledTimeInHours" kind="Identity">
          <setting>
            <aCSMoniker name="/WebApiAzureDeployment/WebApiAzureDeploymentGroup/FilePurgeService/ScheduledTimeInHours" />
          </setting>
        </map>
        <map name="MapFilePurgeService:UploadedFilesExpirationDurationInHours" kind="Identity">
          <setting>
            <aCSMoniker name="/WebApiAzureDeployment/WebApiAzureDeploymentGroup/FilePurgeService/UploadedFilesExpirationDurationInHours" />
          </setting>
        </map>
        <map name="MapFilePurgeServiceInstances" kind="Identity">
          <setting>
            <sCSPolicyIDMoniker name="/WebApiAzureDeployment/WebApiAzureDeploymentGroup/FilePurgeServiceInstances" />
          </setting>
        </map>
        <map name="MapPublishProcessor:DataOnBoardingStorage" kind="Identity">
          <setting>
            <aCSMoniker name="/WebApiAzureDeployment/WebApiAzureDeploymentGroup/PublishProcessor/DataOnBoardingStorage" />
          </setting>
        </map>
        <map name="MapPublishProcessor:Microsoft.Research.DataOnboarding" kind="Identity">
          <setting>
            <aCSMoniker name="/WebApiAzureDeployment/WebApiAzureDeploymentGroup/PublishProcessor/Microsoft.Research.DataOnboarding" />
          </setting>
        </map>
        <map name="MapPublishProcessor:Microsoft.Research.DataOnboarding.QueueService" kind="Identity">
          <setting>
            <aCSMoniker name="/WebApiAzureDeployment/WebApiAzureDeploymentGroup/PublishProcessor/Microsoft.Research.DataOnboarding.QueueService" />
          </setting>
        </map>
        <map name="MapPublishProcessor:Microsoft.Research.DataOnboarding.Services.PublishProcessor" kind="Identity">
          <setting>
            <aCSMoniker name="/WebApiAzureDeployment/WebApiAzureDeploymentGroup/PublishProcessor/Microsoft.Research.DataOnboarding.Services.PublishProcessor" />
          </setting>
        </map>
        <map name="MapPublishProcessor:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" kind="Identity">
          <setting>
            <aCSMoniker name="/WebApiAzureDeployment/WebApiAzureDeploymentGroup/PublishProcessor/Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </setting>
        </map>
        <map name="MapPublishProcessor:PrimaryContainer" kind="Identity">
          <setting>
            <aCSMoniker name="/WebApiAzureDeployment/WebApiAzureDeploymentGroup/PublishProcessor/PrimaryContainer" />
          </setting>
        </map>
        <map name="MapPublishProcessor:ScheduledLogTransferInMinutes" kind="Identity">
          <setting>
            <aCSMoniker name="/WebApiAzureDeployment/WebApiAzureDeploymentGroup/PublishProcessor/ScheduledLogTransferInMinutes" />
          </setting>
        </map>
        <map name="MapPublishProcessor:ScheduledTime" kind="Identity">
          <setting>
            <aCSMoniker name="/WebApiAzureDeployment/WebApiAzureDeploymentGroup/PublishProcessor/ScheduledTime" />
          </setting>
        </map>
        <map name="MapPublishProcessorInstances" kind="Identity">
          <setting>
            <sCSPolicyIDMoniker name="/WebApiAzureDeployment/WebApiAzureDeploymentGroup/PublishProcessorInstances" />
          </setting>
        </map>
        <map name="MapWebApi:DataOnBoardingStorage" kind="Identity">
          <setting>
            <aCSMoniker name="/WebApiAzureDeployment/WebApiAzureDeploymentGroup/WebApi/DataOnBoardingStorage" />
          </setting>
        </map>
        <map name="MapWebApi:Microsoft.Research.DataOnboarding" kind="Identity">
          <setting>
            <aCSMoniker name="/WebApiAzureDeployment/WebApiAzureDeploymentGroup/WebApi/Microsoft.Research.DataOnboarding" />
          </setting>
        </map>
        <map name="MapWebApi:Microsoft.Research.DataOnboarding.FileService" kind="Identity">
          <setting>
            <aCSMoniker name="/WebApiAzureDeployment/WebApiAzureDeploymentGroup/WebApi/Microsoft.Research.DataOnboarding.FileService" />
          </setting>
        </map>
        <map name="MapWebApi:Microsoft.Research.DataOnboarding.FileService.FileProcesser" kind="Identity">
          <setting>
            <aCSMoniker name="/WebApiAzureDeployment/WebApiAzureDeploymentGroup/WebApi/Microsoft.Research.DataOnboarding.FileService.FileProcesser" />
          </setting>
        </map>
        <map name="MapWebApi:Microsoft.Research.DataOnboarding.RepositoryAdapters.SkyDrive" kind="Identity">
          <setting>
            <aCSMoniker name="/WebApiAzureDeployment/WebApiAzureDeploymentGroup/WebApi/Microsoft.Research.DataOnboarding.RepositoryAdapters.SkyDrive" />
          </setting>
        </map>
        <map name="MapWebApi:Microsoft.Research.DataOnboarding.WebApi" kind="Identity">
          <setting>
            <aCSMoniker name="/WebApiAzureDeployment/WebApiAzureDeploymentGroup/WebApi/Microsoft.Research.DataOnboarding.WebApi" />
          </setting>
        </map>
        <map name="MapWebApi:Microsoft.Research.DataOnboarding.WebApi.Api" kind="Identity">
          <setting>
            <aCSMoniker name="/WebApiAzureDeployment/WebApiAzureDeploymentGroup/WebApi/Microsoft.Research.DataOnboarding.WebApi.Api" />
          </setting>
        </map>
        <map name="MapWebApi:Microsoft.Research.DataOnboarding.WebApi.Security" kind="Identity">
          <setting>
            <aCSMoniker name="/WebApiAzureDeployment/WebApiAzureDeploymentGroup/WebApi/Microsoft.Research.DataOnboarding.WebApi.Security" />
          </setting>
        </map>
        <map name="MapWebApi:Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" kind="Identity">
          <setting>
            <aCSMoniker name="/WebApiAzureDeployment/WebApiAzureDeploymentGroup/WebApi/Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" />
          </setting>
        </map>
        <map name="MapWebApi:PrimaryContainer" kind="Identity">
          <setting>
            <aCSMoniker name="/WebApiAzureDeployment/WebApiAzureDeploymentGroup/WebApi/PrimaryContainer" />
          </setting>
        </map>
        <map name="MapWebApiInstances" kind="Identity">
          <setting>
            <sCSPolicyIDMoniker name="/WebApiAzureDeployment/WebApiAzureDeploymentGroup/WebApiInstances" />
          </setting>
        </map>
      </maps>
      <components>
        <groupHascomponents>
          <role name="FilePurgeService" generation="1" functional="0" release="0" software="C:\DataOnboarding\Deployment\WebApiAzureDeployment\csx\Dev\roles\FilePurgeService" entryPoint="base\x64\WaHostBootstrapper.exe" parameters="base\x64\WaWorkerHost.exe " memIndex="1792" hostingEnvironment="consoleroleadmin" hostingEnvironmentVersion="2">
            <settings>
              <aCS name="DataOnBoardingStorage" defaultValue="" />
              <aCS name="Microsoft.Research.DataOnboarding" defaultValue="" />
              <aCS name="Microsoft.Research.DataOnboarding.FilePurgeService" defaultValue="" />
              <aCS name="Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="" />
              <aCS name="PrimaryContainer" defaultValue="" />
              <aCS name="ScheduledTimeInHours" defaultValue="" />
              <aCS name="UploadedFilesExpirationDurationInHours" defaultValue="" />
              <aCS name="__ModelData" defaultValue="&lt;m role=&quot;FilePurgeService&quot; xmlns=&quot;urn:azure:m:v1&quot;&gt;&lt;r name=&quot;FilePurgeService&quot; /&gt;&lt;r name=&quot;PublishProcessor&quot; /&gt;&lt;r name=&quot;WebApi&quot;&gt;&lt;e name=&quot;dataup2httpsin&quot; /&gt;&lt;/r&gt;&lt;/m&gt;" />
            </settings>
            <resourcereferences>
              <resourceReference name="DiagnosticStore" defaultAmount="[4096,4096,4096]" defaultSticky="true" kind="Directory" />
              <resourceReference name="TransientFileStorage" defaultAmount="[32768,32768,32768]" defaultSticky="false" kind="Directory" />
              <resourceReference name="EventStore" defaultAmount="[1000,1000,1000]" defaultSticky="false" kind="LogStore" />
            </resourcereferences>
          </role>
          <sCSPolicy>
            <sCSPolicyIDMoniker name="/WebApiAzureDeployment/WebApiAzureDeploymentGroup/FilePurgeServiceInstances" />
            <sCSPolicyUpdateDomainMoniker name="/WebApiAzureDeployment/WebApiAzureDeploymentGroup/FilePurgeServiceUpgradeDomains" />
            <sCSPolicyFaultDomainMoniker name="/WebApiAzureDeployment/WebApiAzureDeploymentGroup/FilePurgeServiceFaultDomains" />
          </sCSPolicy>
        </groupHascomponents>
        <groupHascomponents>
          <role name="PublishProcessor" generation="1" functional="0" release="0" software="C:\DataOnboarding\Deployment\WebApiAzureDeployment\csx\Dev\roles\PublishProcessor" entryPoint="base\x64\WaHostBootstrapper.exe" parameters="base\x64\WaWorkerHost.exe " memIndex="1792" hostingEnvironment="consoleroleadmin" hostingEnvironmentVersion="2">
            <settings>
              <aCS name="DataOnBoardingStorage" defaultValue="" />
              <aCS name="Microsoft.Research.DataOnboarding" defaultValue="" />
              <aCS name="Microsoft.Research.DataOnboarding.QueueService" defaultValue="" />
              <aCS name="Microsoft.Research.DataOnboarding.Services.PublishProcessor" defaultValue="" />
              <aCS name="Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="" />
              <aCS name="PrimaryContainer" defaultValue="" />
              <aCS name="ScheduledLogTransferInMinutes" defaultValue="" />
              <aCS name="ScheduledTime" defaultValue="" />
              <aCS name="__ModelData" defaultValue="&lt;m role=&quot;PublishProcessor&quot; xmlns=&quot;urn:azure:m:v1&quot;&gt;&lt;r name=&quot;FilePurgeService&quot; /&gt;&lt;r name=&quot;PublishProcessor&quot; /&gt;&lt;r name=&quot;WebApi&quot;&gt;&lt;e name=&quot;dataup2httpsin&quot; /&gt;&lt;/r&gt;&lt;/m&gt;" />
            </settings>
            <resourcereferences>
              <resourceReference name="DiagnosticStore" defaultAmount="[4096,4096,4096]" defaultSticky="true" kind="Directory" />
              <resourceReference name="TransientFileStorage" defaultAmount="[32768,32768,32768]" defaultSticky="false" kind="Directory" />
              <resourceReference name="EventStore" defaultAmount="[1000,1000,1000]" defaultSticky="false" kind="LogStore" />
            </resourcereferences>
          </role>
          <sCSPolicy>
            <sCSPolicyIDMoniker name="/WebApiAzureDeployment/WebApiAzureDeploymentGroup/PublishProcessorInstances" />
            <sCSPolicyUpdateDomainMoniker name="/WebApiAzureDeployment/WebApiAzureDeploymentGroup/PublishProcessorUpgradeDomains" />
            <sCSPolicyFaultDomainMoniker name="/WebApiAzureDeployment/WebApiAzureDeploymentGroup/PublishProcessorFaultDomains" />
          </sCSPolicy>
        </groupHascomponents>
        <groupHascomponents>
          <role name="WebApi" generation="1" functional="0" release="0" software="C:\DataOnboarding\Deployment\WebApiAzureDeployment\csx\Dev\roles\WebApi" entryPoint="base\x64\WaHostBootstrapper.exe" parameters="base\x64\WaIISHost.exe " memIndex="1792" hostingEnvironment="frontendadmin" hostingEnvironmentVersion="2">
            <componentports>
              <inPort name="dataup2httpsin" protocol="http" portRanges="80" />
            </componentports>
            <settings>
              <aCS name="DataOnBoardingStorage" defaultValue="" />
              <aCS name="Microsoft.Research.DataOnboarding" defaultValue="" />
              <aCS name="Microsoft.Research.DataOnboarding.FileService" defaultValue="" />
              <aCS name="Microsoft.Research.DataOnboarding.FileService.FileProcesser" defaultValue="" />
              <aCS name="Microsoft.Research.DataOnboarding.RepositoryAdapters.SkyDrive" defaultValue="" />
              <aCS name="Microsoft.Research.DataOnboarding.WebApi" defaultValue="" />
              <aCS name="Microsoft.Research.DataOnboarding.WebApi.Api" defaultValue="" />
              <aCS name="Microsoft.Research.DataOnboarding.WebApi.Security" defaultValue="" />
              <aCS name="Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString" defaultValue="" />
              <aCS name="PrimaryContainer" defaultValue="" />
              <aCS name="__ModelData" defaultValue="&lt;m role=&quot;WebApi&quot; xmlns=&quot;urn:azure:m:v1&quot;&gt;&lt;r name=&quot;FilePurgeService&quot; /&gt;&lt;r name=&quot;PublishProcessor&quot; /&gt;&lt;r name=&quot;WebApi&quot;&gt;&lt;e name=&quot;dataup2httpsin&quot; /&gt;&lt;/r&gt;&lt;/m&gt;" />
            </settings>
            <resourcereferences>
              <resourceReference name="DiagnosticStore" defaultAmount="[4096,4096,4096]" defaultSticky="true" kind="Directory" />
              <resourceReference name="TransientFileStorage" defaultAmount="[32768,32768,32768]" defaultSticky="false" kind="Directory" />
              <resourceReference name="EventStore" defaultAmount="[1000,1000,1000]" defaultSticky="false" kind="LogStore" />
            </resourcereferences>
          </role>
          <sCSPolicy>
            <sCSPolicyIDMoniker name="/WebApiAzureDeployment/WebApiAzureDeploymentGroup/WebApiInstances" />
            <sCSPolicyUpdateDomainMoniker name="/WebApiAzureDeployment/WebApiAzureDeploymentGroup/WebApiUpgradeDomains" />
            <sCSPolicyFaultDomainMoniker name="/WebApiAzureDeployment/WebApiAzureDeploymentGroup/WebApiFaultDomains" />
          </sCSPolicy>
        </groupHascomponents>
      </components>
      <sCSPolicy>
        <sCSPolicyUpdateDomain name="WebApiUpgradeDomains" defaultPolicy="[5,5,5]" />
        <sCSPolicyUpdateDomain name="FilePurgeServiceUpgradeDomains" defaultPolicy="[5,5,5]" />
        <sCSPolicyUpdateDomain name="PublishProcessorUpgradeDomains" defaultPolicy="[5,5,5]" />
        <sCSPolicyFaultDomain name="FilePurgeServiceFaultDomains" defaultPolicy="[2,2,2]" />
        <sCSPolicyFaultDomain name="PublishProcessorFaultDomains" defaultPolicy="[2,2,2]" />
        <sCSPolicyFaultDomain name="WebApiFaultDomains" defaultPolicy="[2,2,2]" />
        <sCSPolicyID name="FilePurgeServiceInstances" defaultPolicy="[1,1,1]" />
        <sCSPolicyID name="PublishProcessorInstances" defaultPolicy="[1,1,1]" />
        <sCSPolicyID name="WebApiInstances" defaultPolicy="[1,1,1]" />
      </sCSPolicy>
    </group>
  </groups>
  <implements>
    <implementation Id="b98796e9-ff5f-4100-a2e2-ac43a42485b6" ref="Microsoft.RedDog.Contract\ServiceContract\WebApiAzureDeploymentContract@ServiceDefinition">
      <interfacereferences>
        <interfaceReference Id="b812edb1-9573-462e-9c16-f2e07e1a48e1" ref="Microsoft.RedDog.Contract\Interface\WebApi:dataup2httpsin@ServiceDefinition">
          <inPort>
            <inPortMoniker name="/WebApiAzureDeployment/WebApiAzureDeploymentGroup/WebApi:dataup2httpsin" />
          </inPort>
        </interfaceReference>
      </interfacereferences>
    </implementation>
  </implements>
</serviceModel>