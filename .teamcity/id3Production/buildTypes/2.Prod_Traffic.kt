package id3Production.buildTypes

import jetbrains.buildServer.configs.kotlin.v2019_2.*

object id3Production_2ProductionUpdateTrafficWeightManual : BuildType({
    templates(AbsoluteId("PiperUpdateTrafficWeight"))
    id("Prod_Traffic")
    name = "2. [Production] Update Traffic Weight [Manual]"

    buildNumberPattern = "${id3Production_1ProductionDeployAndSmokeTestManual.depParamRefs["env.APP_VERSION"]}"

    params {
        param("env.TARGET_SLICE", "${id3Production_1ProductionDeployAndSmokeTestManual.depParamRefs["env.TARGET_SLICE"]}")
        param("env.TARGET_SLICE_DNS_NAME", "${id3Production_1ProductionDeployAndSmokeTestManual.depParamRefs["env.TARGET_SLICE_DNS_NAME"]}")
        param("env.NGINX_UPSTREAM_KEY", "${id3Production_1ProductionDeployAndSmokeTestManual.depParamRefs["env.NGINX_UPSTREAM_KEY"]}")
        param("env.APP_VERSION", "${id3Production_1ProductionDeployAndSmokeTestManual.depParamRefs["env.APP_VERSION"]}")
    }

    dependencies {
        artifacts(id3Production_1ProductionDeployAndSmokeTestManual) {
            cleanDestination = true
            artifactRules = ".teamcity/settings/digest.txt=>./crap"
        }
    }
})
