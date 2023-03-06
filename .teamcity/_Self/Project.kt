package _Self

import jetbrains.buildServer.configs.kotlin.v2019_2.*
import jetbrains.buildServer.configs.kotlin.v2019_2.Project

object Project : Project({

    params {
        param("env.APP_NAME", "PromocodeService")
        param("GIT_REPOSITORY_SOURCE", "git@github.thetrainline.com:DietCode/PromocodeService.git")
        param("env.USE_CONVENTIONAL_CODE_DEPLOY", "true")
        param("env.PACKAGE_TYPE", "Zip")
        param("env.CONFIG_LOCATION", "--config=./Configuration/PromocodeService")
    }

    subProject(id2Integration.Project)
    subProject(id4Scans.Project)
    subProject(id3Production.Project)
    subProject(id1Development.Project)
})
