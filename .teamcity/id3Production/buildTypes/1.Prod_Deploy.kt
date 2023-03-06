package id3Production.buildTypes

import jetbrains.buildServer.configs.kotlin.v2019_2.*

object id3Production_1ProductionDeployAndSmokeTestManual : BuildType({
    templates(AbsoluteId("PiperInstall"))
    id("Prod_Deploy")
    name = "1. [Production] Deploy and Smoke Test [Manual]"
})
