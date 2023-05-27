const email = "tom123";
const password = "Password123!";
const baseUrl = "http://localhost:3000";

describe("task tests", () => {
  const date = `${new Date()}`;
  const taskName = date;
  beforeEach(() => {
    cy.visit(baseUrl + "/login");
    if (cy.contains("Sign in")) {
      //not signed up
      cy.get('input[placeholder="login"]').type(`${email}{enter}`);
      cy.get('input[placeholder="password"]').type(`${password}{enter}`);
      cy.get("div").contains("sign in").click();
    }
  });

  it("correct authorization", function () {
    cy.contains("tom123");
    cy.contains("My tasks");
  });

  it("can add task", function () {
    cy.get("div").contains("add a new task").click();
    cy.get('input[id="description"]').type(`${date}{enter}`);
    cy.get("div").contains("add a task").click();
    cy.contains(taskName);
    cy.get("div").contains("add a task").should("not.exist");
  });

  it("can edit a task", function () {
    cy.get("div").get("[class^=uneditedTask]").contains(taskName).click();
    cy.get("div")
      .get("[class^=uneditedTask]")
      .contains(taskName)
      .should("not.exist");
    cy.get("div").get("[class^=editedTask]").as("taskContainer");
    cy.get("@taskContainer").get("input").eq(0).as("taskDescriptionInput");
    cy.get("@taskDescriptionInput").should("have.value", taskName);
    cy.get("@taskDescriptionInput").type(`123{enter}`);
    cy.get("@taskContainer").get("img").get("[class^=closeBtn]").click();
    cy.get("@taskContainer").should("not.exist");
    cy.get("div")
      .get("[class^=uneditedTask]")
      .contains(`${taskName}123`)
      .should("exist");
    cy.visit(baseUrl);
    cy.get("div")
      .get("[class^=uneditedTask]")
      .contains(`${taskName}123`)
      .should("exist");
  });
  it("can add a comment", function () {});
  it("can delete a task", function () {
    cy.get("div")
      .get("[class^=uneditedTask]")
      .contains(`${taskName}123`)
      .click();
    cy.get("div").get("[class^=editedTask]").as("taskContainer");
    cy.get("@taskContainer").get("img").get("[class^=deleteBtn]").click();
    cy.get("@taskContainer").should("not.exist");
    cy.get("div")
      .get("[class^=uneditedTask]")
      .contains(`${taskName}123`)
      .should("not.exist");
    cy.visit(baseUrl);
    cy.get("div")
      .get("[class^=uneditedTask]")
      .contains(`${taskName}123`)
      .should("not.exist");
  });
  it("can add a repeated task", function () {});
  it("can add a future task", function () {});
});
