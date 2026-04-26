Feature: Practise API health
  As a QA engineer
  I want to ensure the practise API is reachable
  So that downstream UI and E2E flows can rely on it.

  @smoke @api
  Scenario: API root responds without server error
    Given the API client is configured for the practise environment
    When I send a GET request to "/"
    Then the response status code should be less than 500
