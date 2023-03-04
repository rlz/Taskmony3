import { useState } from "react";
import { FilterDivider } from "./filter-divider";
import hrLine from "../../images/hr-line.svg";
import { FilterItem } from "./filter-item";
import { useSearchParams } from "react-router-dom";
import { getCookie } from "../../utils/cookies";
import {
  useQueryParam,
  NumberParam,
  StringParam,
  ArrayParam,
  withDefault,
  BooleanParam,
} from "use-query-params";

export const FilterByTaskType = () => {
  const myId = getCookie("id");
  const [future, setFuture] = useQueryParam("future",BooleanParam);
  const [followed, setFollowed] = useQueryParam("followed",BooleanParam);
  const [assignedByMe,setAssignedByMe] = useQueryParam("assignedByMe",BooleanParam);

  return (
    <>
      <FilterItem
        label="show future tasks"
        checked={future}
        onChange={(value, label) => {
          setFuture(value)
          }}
      />
      <FilterItem
        label="show assigned by me"
        checked={assignedByMe}
        onChange={(value, label) => {
          setAssignedByMe(value)
        }}
      />
      <FilterItem
        label="show followed"
        checked={followed}
        onChange={(value, label) => {
          setFollowed(value)
          }}
      />
    </>
  );
};

export const FilterByArchivedTaskType = () => {
  const [archiveType, setArchiveType] = useQueryParam("archiveType",StringParam);
  if(!archiveType) setArchiveType("completed")
  return (
    <>
      <FilterItem
        label="deleted"
        checked={archiveType == "deleted"}
        onChange={(value, label) => {
          if (value) setArchiveType("deleted") 
          else setArchiveType("completed")
        }}
        radio
      />
      <FilterItem
        label="completed"
        checked={archiveType == "completed"}
        onChange={(value, label) => {
          if (value) setArchiveType("completed")
          else setArchiveType("deleted")
        }}
        radio
      />
    </>
  );
};
