import { useState } from "react";
import { FilterDivider } from "./filter-divider";
import hrLine from "../../images/hr-line.svg";
import { FilterItem } from "./filter-item";
import { useSearchParams } from "react-router-dom";
import { getCookie } from "../../utils/cookies";

export const FilterByTaskType = () => {
  const myId = getCookie("id");
  let [searchParams, setSearchParams] = useSearchParams();
  const future = searchParams.get("future");
  const creator = searchParams.get("creator");
  const assignee = searchParams.get("assignee");
  const followed = searchParams.get("followed");

  return (
    <>
      <FilterItem
        label="show future tasks"
        checked={!future || future == "yes"}
        onChange={(value, label) => {
          if (value) setSearchParams({ future: "yes" });
          else setSearchParams({ future: "no" });
        }}
      />
      <FilterItem
        label="show assigned to me"
        checked={assignee == myId}
        onChange={(value, label) => {
          if (value) setSearchParams({ assignee: myId });
          else setSearchParams({ assignee: "" });
        }}
      />
      <FilterItem
        label="show assigned by me"
        checked={creator == myId}
        onChange={(value, label) => {
          if (value) setSearchParams({ creator: myId });
          else setSearchParams({ creator: "" });
        }}
      />
      <FilterItem
        label="show followed"
        checked={!followed || followed == "yes"}
        onChange={(value, label) => {
          if (value) setSearchParams({ followed: "yes" });
          else setSearchParams({ followed: "no" });
        }}
      />
    </>
  );
};

export const FilterByArchivedTaskType = () => {
  let [searchParams, setSearchParams] = useSearchParams();
  const archiveType = searchParams.get("archiveType");
  if (!archiveType) setSearchParams({ archiveType: "deleted" });
  console.log(archiveType);
  return (
    <>
      <FilterItem
        label="deleted"
        checked={archiveType == "deleted"}
        onChange={(value, label) => {
          if (value) setSearchParams({ archiveType: "deleted" });
        }}
        radio
      />
      <FilterItem
        label="completed"
        checked={archiveType == "completed"}
        onChange={(value, label) => {
          if (value) setSearchParams({ archiveType: "completed" });
        }}
        radio
      />
    </>
  );
};
