pipeline {
    agent any

    environment {
        SOLUTION = 'Acme.BookStore.sln'
        BUILD_CONFIGURATION = 'Release'
        ARTIFACT_NAME = "AcmeBookStore-${env.BUILD_NUMBER}"
        NPM_CACHE_FOLDER = "${env.WORKSPACE}\\.npm"
        ARTIFACT_DIR = "${env.WORKSPACE}\\publish"
    }

    stages {
        
        stage('Restore Dependencies') {
            steps {
                echo "Restoring solution dependencies..."
                bat "dotnet restore ${SOLUTION}"
            }
        }

        stage('Build') {
            steps {
                echo "Building solution..."
                bat "dotnet build ${SOLUTION} --configuration ${BUILD_CONFIGURATION} --no-restore"
            }
        }

        stage('Publish') {
            steps {
                echo "Publishing solution..."
                bat "dotnet publish ${SOLUTION} --configuration ${BUILD_CONFIGURATION} --output ${ARTIFACT_DIR} --no-build"
                bat "powershell Compress-Archive -Path ${ARTIFACT_DIR}\\* -DestinationPath ${ARTIFACT_NAME}.zip"
            }
        }

        stage('Archive Artifacts') {
            steps {
                archiveArtifacts artifacts: "${ARTIFACT_NAME}.zip", fingerprint: true
            }
        }
        stage('Clean Workspace') {
            steps {
                cleanWs() // Jenkins built-in workspace cleaner
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
