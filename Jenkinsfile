pipeline {
    agent any // Use any available agent

    parameters {
        string(name: 'PROJECT_KEY', description: 'Key of the project', defaultValue: 'T1')
        string(name: 'TEST_EXECUTION_KEY', description: 'Key of the test execution', defaultValue: 'T1-3879')
        string(name: 'testEnvironments', description: 'Key of the project', defaultValue: 'stg')
        string(name: 'milestoneId', description: 'Key of the test execution', defaultValue: '3214')
        string(name: 'fixVersions', description: 'Key of the project', defaultValue: '1234')
        string(name: 'testPlanKeys', description: 'Key of the test execution', defaultValue: 'T1-68')
        
    }

    environment {
        CLIENT_ID = 'ohuKO8VOiFh/OeeK+qyP6xz/l7z1nivhqrkAA9BvBnI71Lbv30ZrgYM5hf4a+6v+'
        CLIENT_SECRET = 'af30354960b288f964a3405547ad0510981ef366b3e592972c4213ce8cda391e'
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
        curl -s 'https://agiletest.atlas.devsamurai.com/api/apikeys/authenticate' -X POST -H 'Content-Type:application/json' \
        --data '{"clientId":"'"$env.CLIENT_ID"'", "clientSecret":"'"$env.CLIENT_SECRET"'"}' \
        | tr -d '"'
    """, returnStdout: true).trim()
}

def submitTestResults(token) {
    return sh(script: """
        curl -X POST -H "Content-Type: application/xml" \
        -H "Authorization: JWT ${token}" \
        --data @"./playwright-report/results.xml" \
        "https://api.agiletest.app/ds/test-executions/junit?projectKey=${{ params.PROJECT_KEY }}&testExecutionKey=${{ params.TEST_EXECUTION_KEY }}&milestoneId=${{ params.milestoneId }}&testEnvironments=${{ params.testEnvironments }}&testPlanKeys=${{ params.testPlanKeys }}&revision=${{ params.revision }}&fixVersions=${{ params.fixVersions }}"
    """, returnStdout: true).trim()
}

def sendBuildStatus(token, status) {
    def response = sh(script: """
        curl -s -H "Content-Type:application/json" -H "Authorization:JWT $token" \
        --data '{ "buildURL": "'"$env.BUILD_URL"'", "tool":"jenkins-multibranch", "result":"${status}" }' \
        "https://agiletest.atlas.devsamurai.com/ds/test-executions/${params.TEST_EXECUTION_KEY}/pipeline/history?projectKey=${params.PROJECT_KEY}"
    """, returnStdout: true).trim()

    echo "API Response for ${status} build: ${response}"
}
