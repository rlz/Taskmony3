import { useState } from "react";
import { AddBtn } from "../../components/add-btn/add-btn";
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
        <Idea label={"idea #3"} followed direction="Taskmony" />
      </div>
      <Filter />
    </div>
  );
}

function Filter() {
  const [isOpen1, setIsOpen1] = useState<boolean>(true);
  const [isOpen2, setIsOpen2] = useState<boolean>(true);
  return (
    <div className="w-1/5 mt-12">
      <FilterDivider
        isOpen={isOpen1}
        setIsOpen={setIsOpen1}
        title="filter by category"
      />
      {isOpen1 && (
        <>
          <FilterItem label="hot" checked radio/>
          <FilterItem label="later" checked={false} radio/>
          <FilterItem label="too good to delete" checked={false} radio/>
        </>
      )}
      <hr/>
      <FilterItem label="show followed" checked />
      <hr/>
      <FilterDivider
        isOpen={isOpen2}
        setIsOpen={setIsOpen2}
        title="filter by direction"
      />
      {isOpen2 && (
        <>
          <FilterItem label="Project #1" checked />
          <FilterItem label="Project #1" checked />
          <FilterItem label="Project #1" checked />
        </>
      )}
    </div>
  );
}

export default MyIdeas;
