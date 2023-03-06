package id3Production

import id3Production.buildTypes.*
import jetbrains.buildServer.configs.kotlin.v2019_2.*
import jetbrains.buildServer.configs.kotlin.v2019_2.Project

object Project : Project({
    id("3Production")
    name = "3. Production"

    buildType(id3Production_2ProductionUpdateTrafficWeightManual)
    buildType(id3Production_1ProductionDeployAndSmokeTestManual)
    buildType(id3Production_3ProductionPromoteManual)
    buildType(id3Production_4ProductionRollbackManual)
})
