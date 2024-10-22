pipeline {
    agent any

    parameters {
        string(name: 'PROJECT_KEY', description: 'The key of the project', defaultValue: 'AUT')
        string(name: 'TEST_EXECUTION_KEY', description: 'The key of the test execution', defaultValue: 'AUT-3879')
    }

    environment {
        CLIENT_ID = 'Mmar0YgnY3LFQD7I3AqlwEQ95xJ1i0Le0GVy49f1wcc='
        CLIENT_SECRET = 'dc6c48806069f4f8c2442076bdc806cc81170aa9aefa91a51eff979e5515b5d7'
    }

    stages {
        stage('Checkout Code') {
            steps {
                // Replace with your Git repository URL
                checkout scm
            }
        }

        stage('Run Tests') {
            steps {
                script {
                    echo "Running tests..."
                    // Navigate to the directory containing your tests
                    dir('/Users/thuydung/Desktop/gitlab/agiletest2') {
                        // Run npm tests and allow failure
                        sh 'npm test || true'
                    }
                    echo "Tests completed."
                }
            }
        }

        stage('Authenticate and Upload Results') {
            steps {
                script {
                    // Authenticate and get the token
                    def response = sh(script: """
                        curl -X POST 'https://dev.agiletest.atlas.devsamurai.com/api/apikeys/authenticate' \
                        -H 'Content-Type: application/json' \
                        --data '{"clientId":"${env.CLIENT_ID}","clientSecret":"${env.CLIENT_SECRET}"}'
                    """, returnStdout: true).trim()

                    // Extract token from response
                    def token = response.token // Modify this if response parsing needs adjustment
                    echo "Token: ${token}"

                    // Upload results
                    def uploadResponse = sh(script: """
                        curl -X POST -H "Content-Type: application/xml" \
                        -H "Authorization: JWT ${token}" \
                        --data @"./playwright-report/results.xml" \
                        "https://dev.api.agiletest.app/ds/test-executions/junit?projectKey=${params.PROJECT_KEY}&testExecutionKey=${params.TEST_EXECUTION_KEY}"
                    """, returnStdout: true).trim()

                    echo "API Response: ${uploadResponse}"
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