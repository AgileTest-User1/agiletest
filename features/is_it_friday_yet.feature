Feature: Is it Friday yet?
  Everybody wants to know when it's Friday

@AUT-742
  Scenario: Sunday isn't Friday
    Given today is Sunday
    When I ask whether it's Friday yet
    Then I should be told "Nope"