import { useState } from "react";
import { AddBtn } from "../../components/add-btn/add-btn";
import { FilterByDirection } from "../../components/filter/by-direction";
import { FilterByFollowed, FilterByIdeaCategory } from "../../components/filter/by-idea-category";
import { FilterDivider } from "../../components/filter/filter-divider";
import { FilterItem } from "../../components/filter/filter-item";

import { Idea } from "../../components/idea";

function MyIdeas() {
  return (
    <div className="flex w-full">
      <div className="w-3/4  m-3">
        <h1 className="font-bold text-3xl">My Ideas</h1>
        <AddBtn label={"add a new idea"} onClick={() => {}} />
        <Idea
          label={"idea #1"}
          comments={1}
          createdBy="Ann Smith"
          direction="Taskmony"
        />
        <Idea label={"idea #2"} direction="Taskmony" />
        <Idea label={"idea #3"} followed direction="Taskmony" last />
      </div>
      <Filter />
    </div>
  );
}

function Filter() {
  return (
    <div className="w-1/5 mt-12">
      <FilterByIdeaCategory />
      <hr />
      <FilterByFollowed />
      <hr />
      <FilterByDirection />
    </div>
  );
}

export default MyIdeas;
