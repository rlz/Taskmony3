const email = "tom123";
const password = "Password123!";
const baseUrl = "http://localhost:3000";

describe("ideas tests", () => {
  const date = `${new Date()}`;
  const ideaName = date;
  beforeEach(() => {
    cy.visit(baseUrl + "/login");
    if (cy.contains("Sign in")) {
      //not signed up
      cy.get('input[placeholder="login"]').type(`${email}{enter}`);
      cy.get('input[placeholder="password"]').type(`${password}{enter}`);
      cy.get("div").contains("sign in").click();
    }
    cy.contains("tom123");
    cy.contains("My tasks");
    cy.visit(baseUrl + "/ideas");
  });

  it("correct authorization", function () {
    cy.contains("tom123");
    cy.contains("My ideas");
  });

  it("can add idea", function () {
    cy.get("div").contains("add a new idea").click();
    cy.get('input[id="description"]').type(`${date}{enter}`);
    cy.get("div").contains("add an idea").click();
    cy.contains(ideaName);
    cy.get("div").contains("add an idea").should("not.exist");
  });

  it("can edit a idea", function () {
    cy.get("div").get("[class^=uneditedIdea]").contains(ideaName).click();
    cy.get("div")
      .get("[class^=uneditedIdea]")
      .contains(ideaName)
      .should("not.exist");
    cy.get("div").get("[class^=editedIdea]").as("ideaContainer");
    cy.get("@ideaContainer").get("input").eq(0).as("ideaDescriptionInput");
    cy.get("@ideaDescriptionInput").should("have.value", ideaName);
    cy.get("@ideaDescriptionInput").type(`123{enter}`);
    cy.get("@ideaContainer").get("img").get("[class^=closeBtn]").click();
    cy.get("@ideaContainer").should("not.exist");
    cy.get("div")
      .get("[class^=uneditedIdea]")
      .contains(`${ideaName}123`)
      .should("exist");
    cy.visit(baseUrl + "/ideas");
    cy.get("div")
      .get("[class^=uneditedIdea]")
      .contains(`${ideaName}123`)
      .should("exist");
  });
  it("can add a comment", function () {});
  it("can delete a idea", function () {
    cy.get("div")
      .get("[class^=uneditedIdea]")
      .contains(`${ideaName}123`)
      .click();
    cy.get("div").get("[class^=editedIdea]").as("ideaContainer");
    cy.get("@ideaContainer").get("img").get("[class^=deleteBtn]").click();
    cy.get("@ideaContainer").should("not.exist");
    cy.get("div")
      .get("[class^=uneditedIdea]")
      .contains(`${ideaName}123`)
      .should("not.exist");
    cy.visit(baseUrl + "/ideas");
    cy.get("div")
      .get("[class^=uneditedIdea]")
      .contains(`${ideaName}123`)
      .should("not.exist");
  });
});
