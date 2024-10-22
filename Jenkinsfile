pipeline {
    agent any // Use any available agent

    parameters {
        string(name: 'PROJECT_KEY', description: 'The key of the project', defaultValue: 'AUT')
        string(name: 'TEST_EXECUTION_KEY', description: 'The key of the test execution', defaultValue: 'AUT-3879')
    }

    environment {
        CLIENT_ID = 'Mmar0YgnY3LFQD7I3AqlwEQ95xJ1i0Le0GVy49f1wcc='
        CLIENT_SECRET = 'dc6c48806069f4f8c2442076bdc806cc81170aa9aefa91a51eff979e5515b5d7'
        // Assuming Node.js is installed in /usr/local/bin or similar
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
                    dir('/Users/thuydung/Desktop/gitlab/agiletest2') {
                        sh 'npm ci' // Install dependencies
                        sh 'npm test || true' // Run tests
                        sh 'ls' // Run tests
                    }
                    echo "Tests completed."
                }
            }
        }

 stage('API Call') {
            steps {
                script {
                    def token = sh(script: '''
                        curl 'https://dev.agiletest.atlas.devsamurai.com/api/apikeys/authenticate' -X POST -H 'Content-Type:application/json' --data '{"clientId":"${env.CLIENT_ID}","clientSecret":"${env.CLIENT_SECRET}"}' | tr -d '"'
                    ''', returnStdout: true).trim()
                    echo "API Token: ${token}"

                    echo "Project key: $PROJECT_KEY"
                    echo "Test execution key: $TEST_EXECUTION_KEY"

                    def response = sh(script: """
curl -X POST -H "Content-Type: application/xml" \
                        -H "Authorization: JWT ${token}" \
                        --data @"./playwright-report/results.xml" \
                        "https://dev.api.agiletest.app/ds/test-executions/junit?projectKey=${params.PROJECT_KEY}&testExecutionKey=${params.TEST_EXECUTION_KEY}"
                    """, returnStdout: true).trim()
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
}
