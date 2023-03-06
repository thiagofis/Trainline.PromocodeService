package id4Scans.buildTypes

import jetbrains.buildServer.configs.kotlin.v2019_2.*
import jetbrains.buildServer.configs.kotlin.v2019_2.buildFeatures.commitStatusPublisher

object id4Scans_1SonarQube : BuildType({
    templates(AbsoluteId("PiperSonarQube"))
    id("SonarQube")
    name = "1. SonarQube [On every commit to master]"
    description = "[On every commit to master]"

    allowExternalStatus = true

    steps {
        step {
            name = "Begin SonarQube"
            id = "RUNNER_246"
            type = "sonar-qube-msbuild"
            param("sonarProjectName", "DietCode_%env.SQUAD_NAME%_%env.APP_NAME%")
            param("additionalParameters", """
                "/d:sonar.cs.dotcover.reportsPaths=%teamcity.build.checkoutDir%\AppCoverageReport.html"
                "/d:sonar.msbuild.testProjectPattern=.*Tests.*?\.(cs|vb)proj${'$'}"
                "/d:sonar.exclusions=*Contracts*,*TestHelpers*
            """.trimIndent())
            param("teamcity.tool.sonarqubemsbuild", "%teamcity.tool.sonar-scanner-msbuild.DEFAULT%")
            param("sonarProjectKey", "DietCode_%env.APP_NAME%")
            param("sonarServer", "aece19d8-886d-44d0-abfc-45faca7d82e9")
        }
        stepsOrder = arrayListOf("RUNNER_246", "RUNNER_559", "RUNNER_247")
    }

    features {
        commitStatusPublisher {
            id = "BUILD_EXT_16"
            vcsRootExtId = "SourceMaster"
            publisher = github {
                githubUrl = "https://github.thetrainline.com/api/v3"
                authType = personalToken {
                    token = "zxx16775821422a3824301940fd63f76fdef33d69da835466ea9b3830da757169131bc239ce4d97c6a0775d03cbe80d301b"
                }
            }
        }
    }
    
    disableSettings("RUNNER_243", "RUNNER_468")
})
