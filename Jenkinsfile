pipeline {
    agent any

    environment {
        SOLUTION = '**/*.csproj'
        BUILD_CONFIGURATION = 'Release'
        ARTIFACT_NAME = 'dotnet9app'
        NPM_CACHE_FOLDER = "${env.WORKSPACE}/.npm"
        ARTIFACT_DIR = "${env.WORKSPACE}/publish"
    }

    stages {

        stage('Restore Dependencies') {
            steps {
                echo "Restoring projects: ${env.SOLUTION}"
                sh "dotnet restore ${env.SOLUTION}"
            }
        }

        stage('Build') {
            steps {
                echo "Building with configuration: ${env.BUILD_CONFIGURATION}"
                sh "dotnet build ${env.SOLUTION} --configuration ${env.BUILD_CONFIGURATION}"
            }
        }

        stage('Publish') {
            steps {
                echo "Publishing projects..."
                sh "dotnet publish ${env.SOLUTION} --configuration ${env.BUILD_CONFIGURATION} --output ${env.ARTIFACT_DIR} /p:PublishSingleFile=true"
                // Optional zip step
                sh "zip -r ${env.ARTIFACT_NAME}.zip ${env.ARTIFACT_DIR}"
            }
        }

        stage('Archive Artifacts') {
            steps {
                archiveArtifacts artifacts: "${ARTIFACT_NAME}.zip", fingerprint: true
            }
        }
    }

    post {
        failure {
            echo 'Build failed.'
        }
        success {
            echo 'Build succeeded.'
        }
    }
}
