package id4Scans.buildTypes

import jetbrains.buildServer.configs.kotlin.v2019_2.*
import jetbrains.buildServer.configs.kotlin.v2019_2.triggers.vcs

object id4Scans_2CheckmarxScan : BuildType({
    templates(AbsoluteId("PiperCheckmarxMaster"))
    id("Checkmarx")
    name = "2. Checkmarx Scan [On every commit to master]"

    params {
        param("env.APP_NAME", "%env.CHECKMARX_TEAM_PREFIX%Trainline.PromocodeService")
    }

    triggers {
        vcs {
            id = "vcsTrigger"
        }
    }
})
