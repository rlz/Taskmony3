import { useState } from "react";
import { AddBtn } from "../../components/add-btn/add-btn";
import { FilterByCreator } from "../../components/filter/by-creator";
import { FilterByIdeaCategory } from "../../components/filter/by-idea-category";
import { FilterDivider } from "../../components/filter/filter-divider";
import { FilterItem } from "../../components/filter/filter-item";

import { Idea } from "../../components/idea";

function Ideas({ directionId }) {
  return (
    <div className="flex w-full">
      <div className="w-3/4 m-3 ml-0">
        <AddBtn label={"add a new idea"} onClick={() => {}} />
        <Idea
          label={"idea #1"}
          comments={1}
          createdBy="Ann Smith"
          direction="Taskmony"
          followed={false}
        />
        <Idea label={"idea #2"} direction="Taskmony" followed={false} />
        <Idea label={"idea #3"} followed direction="Taskmony" last />
      </div>
      <Filter directionId={directionId} />
    </div>
  );
}

function Filter({ directionId }) {
  return (
    <div className="w-1/5 mt-4">
      <FilterByIdeaCategory />
      <hr />
      <FilterItem label="show followed" checked />
      <hr />
      <FilterByCreator id={directionId} />
    </div>
  );
}

export default Ideas;
