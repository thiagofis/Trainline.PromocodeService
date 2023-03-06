package id3Production.buildTypes

import jetbrains.buildServer.configs.kotlin.v2019_2.*

object id3Production_4ProductionRollbackManual : BuildType({
    templates(AbsoluteId("PiperRollback"))
    id("Prod_Rollback")
    name = "4. [Production] Rollback [Manual]"

    buildNumberPattern = "${id3Production_1ProductionDeployAndSmokeTestManual.depParamRefs["env.APP_VERSION"]} (${id3Production_1ProductionDeployAndSmokeTestManual.depParamRefs["env.TARGET_SLICE"]}: 0%)"

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
