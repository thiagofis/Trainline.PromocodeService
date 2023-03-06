package id4Scans.buildTypes

import jetbrains.buildServer.configs.kotlin.v2019_2.*
import jetbrains.buildServer.configs.kotlin.v2019_2.triggers.schedule

object id4Scans_2CheckmarxScanOsa : BuildType({
    templates(AbsoluteId("PiperCheckmarxMaster"))
    id("CheckmarxOsa")
    name = "3. Checkmarx Scan - Include External Dependency Scan [weekly]"

    params {
        param("env.APP_NAME", "%env.CHECKMARX_TEAM_PREFIX%Trainline.PromocodeService")
        param("env.CHECKMARX_ENABLEOSA", "true")
    }

    triggers {
        schedule {
            id = "TRIGGER_61"
            schedulingPolicy = weekly {
                hour = 9
            }
            triggerBuild = always()
        }
    }
})
