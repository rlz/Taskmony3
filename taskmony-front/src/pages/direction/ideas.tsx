import { useState } from "react";
import { AddBtn } from "../../components/add-btn/add-btn";
import { FilterDivider } from "../../components/filter/filter-divider";
import { FilterItem } from "../../components/filter/filter-item";

import { Idea } from "../../components/idea";

function Ideas() {
  return (
    <div className="flex w-full">
      <div className="w-3/4  m-3">
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
    <div className="w-1/5 mt-28">
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
      <FilterDivider isOpen={isOpen2} setIsOpen={setIsOpen2} title="filter by creator" />
      {isOpen2 && (
        <>
          <FilterItem label="all" checked />
          <FilterItem label="me" checked/>
          <FilterItem label="user #1" checked />
        </>
      )}
    </div>
  );
}

export default Ideas;
