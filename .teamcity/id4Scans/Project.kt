package id4Scans

import id4Scans.buildTypes.*
import jetbrains.buildServer.configs.kotlin.v2019_2.*
import jetbrains.buildServer.configs.kotlin.v2019_2.Project

object Project : Project({
    id("4Scans")
    name = "4. Scans"
    description = "SonarQube | CheckMarx"

    buildType(id4Scans_2CheckmarxScanOsa)
    buildType(id4Scans_2CheckmarxScan)
    buildType(id4Scans_1SonarQube)
    buildTypesOrder = arrayListOf(id4Scans_1SonarQube, id4Scans_2CheckmarxScan, id4Scans_2CheckmarxScanOsa)
})
