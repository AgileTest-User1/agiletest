pipeline {
    agent any // Use any available agent

    parameters {
        string(name: 'PROJECT_KEY', description: 'Key of the project', defaultValue: 'AUT')
        string(name: 'TEST_EXECUTION_KEY', description: 'Key of the test execution', defaultValue: 'AUT-3879')
    }

    environment {
        CLIENT_ID = 'Mmar0YgnY3LFQD7I3AqlwEQ95xJ1i0Le0GVy49f1wcc='
        CLIENT_SECRET = 'dc6c48806069f4f8c2442076bdc806cc81170aa9aefa91a51eff979e5515b5d7'
        PATH = "/usr/local/bin:${env.PATH}" // Add Node.js to PATH
    }

    stages {
        stage('Checkout Code') {
            steps {
                checkout scm
            }
        }

        stage('Run Tests') {
            steps {
                script {
                    echo "Running tests..."
                    dir('/Users/thuydung/Desktop/github/agiletest') {
                        sh 'npm ci' // Install dependencies
                        sh 'npm run' // Run tests
                    }
                    echo "Tests completed."
                }
            }
        }

        stage('API Authentication & Test Result Submission') {
            steps {
                script {
                    def token = authenticateApi()
                    echo "API Token: ${token}"

                    echo "Project key: $PROJECT_KEY"
                    echo "Test execution key: $TEST_EXECUTION_KEY"

                    def response = submitTestResults(token)
                    echo "API Response: ${response}"
                }
            }
        }

        stage('Finish') {
            steps {
                echo "Build process completed."
            }
        }
    }

    post {
        success {
            script {
                echo 'Build succeeded.'
                def token = authenticateApi()
                sendBuildStatus(token, "success")
            }
        }
        failure {
            script {
                echo 'Build failed.'
                def token = authenticateApi()
                sendBuildStatus(token, "failed")
            }
        }
    }
}

def authenticateApi() {
    return sh(script: """
        curl -s 'https://dev.agiletest.atlas.devsamurai.com/api/apikeys/authenticate' -X POST -H 'Content-Type:application/json' \
        --data '{"clientId":"'"$env.CLIENT_ID"'", "clientSecret":"'"$env.CLIENT_SECRET"'"}' \
        | tr -d '"'
    """, returnStdout: true).trim()
}

def submitTestResults(token) {
    return sh(script: """
        curl -X POST -H "Content-Type: application/xml" \
        -H "Authorization: JWT ${token}" \
        --data @"./playwright-report/results.xml" \
        "https://dev.api.agiletest.app/ds/test-executions/junit?projectKey=${params.PROJECT_KEY}&testExecutionKey=${params.TEST_EXECUTION_KEY}"
    """, returnStdout: true).trim()
}

def sendBuildStatus(token, status) {
    def response = sh(script: """
        curl -s -H "Content-Type:application/json" -H "Authorization:JWT $token" \
        --data '{ "buildURL": "'"$env.BUILD_URL"'", "tool":"jenkins-multibranch", "result":"${status}" }' \
        "https://dev.agiletest.atlas.devsamurai.com/ds/test-executions/${params.TEST_EXECUTION_KEY}/pipeline/history?projectKey=${params.PROJECT_KEY}"
    """, returnStdout: true).trim()

    echo "API Response for ${status} build: ${response}"
}
