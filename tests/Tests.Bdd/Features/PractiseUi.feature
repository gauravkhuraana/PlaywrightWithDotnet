Feature: Practise UI smoke
  As a QA engineer
  I want to verify the practise UI scenarios page loads
  So that UI flows can run reliably.

  @smoke @ui
  Scenario: Scenarios page is reachable
    Given a fresh browser session is launched
    When I navigate to the scenarios page
    Then the page URL should contain "scenarios"
