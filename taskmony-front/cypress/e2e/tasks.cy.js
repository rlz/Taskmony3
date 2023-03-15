const email = "tom123";
const password = "Password123!";
const baseUrl = "http://localhost:3000";

describe("task tests", () => {
  const taskName = `test${new Date()}`;
  beforeEach(() => {
    cy.visit(baseUrl + "/login");
    if (cy.contains("Sign in")) {
      //not signed up
      cy.get('input[placeholder="login"]').type(`${email}{enter}`);
      cy.get('input[placeholder="password"]').type(`${password}{enter}`);
      cy.get("div").contains("sign in").click();
    }
  });

  it("should be on main page", function () {
    cy.contains("tom123");
    cy.contains("My tasks");
  });

  it("can add task", function () {
    
    cy.get("div").contains("add a new task").click();
    cy.get('input[placeholder="task name"]').type(`${taskName}{enter}`);
    cy.get("div").contains("add a task").click();
    cy.contains(taskName);
    cy.get("div").contains("add a task").should("not.exist");
  });

  it("can edit a task", function () {
    cy.get("div").get('[class^=uneditedTask]').contains(taskName).click();
    cy.get("div").get('[class^=uneditedTask]').contains(taskName).should("not.exist");
    cy.get("div").get('[class^=editedTask]').as("taskContainer")
    cy.get("@taskContainer").get("input").eq(0).as("taskDescriptionInput")
    cy.get("@taskDescriptionInput").should('have.value', taskName);
    cy.get("@taskDescriptionInput").type(`123{enter}`);
    cy.get("@taskContainer").get('img').get('[class^=closeBtn]').click();
    cy.get("@taskContainer").should("not.exist");
    cy.get("div").get('[class^=uneditedTask]').contains(`${taskName}123`).should("exist");
    cy.visit(baseUrl);
    cy.get("div").get('[class^=uneditedTask]').contains(`${taskName}123`).should("exist");
  });
  it("can delete a task", function () {
    cy.get("div").get('[class^=uneditedTask]').contains(`${taskName}123`).click();
    cy.get("div").get('[class^=editedTask]').as("taskContainer")
    cy.get("@taskContainer").get('img').get('[class^=deleteBtn]').click();
    cy.get("@taskContainer").should("not.exist");
    cy.get("div").get('[class^=uneditedTask]').contains(`${taskName}123`).should("not.exist");
    cy.visit(baseUrl);
    cy.get("div").get('[class^=uneditedTask]').contains(`${taskName}123`).should("not.exist");
  });

  
});
