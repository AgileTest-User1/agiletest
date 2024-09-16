module.exports = {
    default: {
      require: ["cucumber/features/steps/**/*.js"],
      format: [
        "progress", // Progress bar in the console
        "json:reports/cucumber-report.json",
      ],
      paths: ["cucumber/features/**/*.feature"],
      formatOptions: {
        snippetInterface: "async-await", // Use async/await in generated step definitions
      },
    },
  };