package id2Integration.buildTypes

import jetbrains.buildServer.configs.kotlin.v2019_2.*

object id2Integration_6PushReleasePackageManual : BuildType({
    templates(AbsoluteId("PiperPromoteToStable"))
    id("ReleasePackage")
    name = "6. Push Release Package [Manual]"

    params {
        param("env.STABLE_VERSION", "")
    }

    dependencies {
        dependency(id2Integration_1BuildTestPackageAndPushReleaseCandida2) {
            snapshot {
                onDependencyFailure = FailureAction.FAIL_TO_START
            }

            artifacts {
                id = "ARTIFACT_DEPENDENCY_107"
                artifactRules = """
                    Artifacts => ./temp/Artifacts
                    pipespec.yml => .
                    .semver => .
                """.trimIndent()
            }
        }
    }
})
