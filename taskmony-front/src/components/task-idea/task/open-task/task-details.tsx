import { useState } from "react";
import {
  changeTaskAssignee,
  changeTaskDirection,
  changeTaskRepeatMode,
  changeTaskRepeatUntil,
  changeTaskStartAt,
  CHANGE_TASK_ASSIGNEE,
  CHANGE_TASK_DIRECTION,
  CHANGE_TASK_REPEAT_EVERY,
  CHANGE_TASK_REPEAT_MODE,
  CHANGE_TASK_REPEAT_UNTIL,
  CHANGE_TASK_REPEAT_WEEK_DAYS,
  CHANGE_TASK_START_DATE,
} from "../../../../services/actions/tasksAPI";
import { useAppDispatch, useAppSelector } from "../../../../utils/hooks";
import { DatePicker } from "../../open-items-components/date-picker";
import { ItemPicker } from "../../open-items-components/item-picker";
import { NumberPicker } from "../../open-items-components/number-picker";
import { WeekPicker } from "../../open-items-components/week-picker";
import { ChangeRepeatedValueModal } from "./repeated-modal";
import { useMediaQuery } from "react-responsive";

type DetailsProps = { fromDirection?: string | null };
export const Details = ({ fromDirection }: DetailsProps) => {
  const dispatch = useAppDispatch();
  const task = useAppSelector((store) => store.editedTask);
  const repeatOptions = ["no", "daily", "custom"];
  const directions = useAppSelector((store) => store.directions.items).filter(
    (i) => i.deletedAt == null
  );

  const repeatModeTranslator = (mode: string) => {
    switch (mode) {
      case "no":
        return null;
      case "daily":
        return "DAY";
      case "custom":
        return "WEEK";
      case null:
        return "no";
      case "DAY":
        return "daily";
      case "WEEK":
        return "custom";
      default:
        return null;
    }
    return null;
  };
  const defaultUntilDateWeekly = () => {
    const date = new Date(Date.now());
    return new Date(date.setDate(date.getDate() + 7));
    // return new Date(date.setFullYear(date.getFullYear() + 1));
  };
  const defaultUntilDateDaily = () => {
    const date = new Date(Date.now());
    return new Date(date.setDate(date.getDate() + 1));
    // return new Date(date.setFullYear(date.getFullYear() + 1));
  };
  const members = directions.filter((d) => d.id == task.direction?.id)[0]
    ?.members;
  const [showModal, setShowModal] = useState<string | null>(null);
  const isSmallScreen = useMediaQuery({ query: "(max-width: 640px)" });
  return (
    <div
      className={`gap flex flex-wrap justify-start pb-2 w-full pl-1 ${
        isSmallScreen && "flex-col"
      }`}
    >
      {showModal && (
        <ChangeRepeatedValueModal
          changeThis={() => {
            setShowModal(null);
            if (showModal == "CHANGE_DIRECTION")
              dispatch(changeTaskDirection(task.id, task.direction));
            else if (showModal == "CHANGE_ASSIGNEE")
              dispatch(changeTaskAssignee(task.id, task.assignee));
          }}
          changeAll={() => {
            setShowModal(null);
            if (showModal == "CHANGE_DIRECTION")
              dispatch(
                changeTaskDirection(task.id, task.direction, task.groupId)
              );
            else if (showModal == "CHANGE_ASSIGNEE")
              dispatch(
                changeTaskAssignee(task.id, task.assignee, task.groupId)
              );
          }}
        />
      )}
      {!fromDirection && (
        <ItemPicker
          title={"direction"}
          option={task.direction?.name ? task.direction?.name : "none"}
          options={["none", ...directions.map((dir) => dir.name)]}
          onChange={(index: number) => {
            const payload = index == 0 ? null : directions[index - 1];
            dispatch({ type: CHANGE_TASK_DIRECTION, payload: payload });
            if (task.groupId) {
              setShowModal("CHANGE_DIRECTION");
            } else if (task.id && payload)
              dispatch(changeTaskDirection(task.id, payload));
          }}
          hasBorder
        />
      )}
      {members && (
        <ItemPicker
          title={"assignee"}
          options={members.map((m) => m.displayName)}
          option={
            task.assignee?.displayName ? task.assignee?.displayName : "none"
          }
          onChange={(index: number) => {
            const payload = members[index];
            dispatch({ type: CHANGE_TASK_ASSIGNEE, payload: payload });
            if (task.groupId) {
              setShowModal("CHANGE_ASSIGNEE");
            } else if (task.id) dispatch(changeTaskAssignee(task.id, payload));
          }}
          hasBorder
          width="w-24"
        />
      )}
      <DatePicker
        title={"start date"}
        disabled={!!task.id && task.repeatMode}
        date={task.startAt ? new Date(task.startAt) : new Date()}
        hasBorder
        onChange={(value: string) => {
          dispatch({ type: CHANGE_TASK_START_DATE, payload: value });
          if (task.id) dispatch(changeTaskStartAt(task.id, value));
        }}
      />
      <ItemPicker
        disabled={!!task.id}
        title={"repeated"}
        options={repeatOptions}
        option={repeatModeTranslator(task.repeatMode) || "no"}
        onChange={(index: number) => {
          dispatch({
            type: CHANGE_TASK_REPEAT_MODE,
            payload: repeatModeTranslator(repeatOptions[index]),
          });
          //when mode is weekly
          if (repeatOptions[index] == "custom") {
            dispatch({ type: CHANGE_TASK_REPEAT_EVERY, payload: 1 });
            dispatch({
              type: CHANGE_TASK_REPEAT_UNTIL,
              payload: new Date(defaultUntilDateWeekly())
                .toISOString()
                .substring(0, 10),
            });
          }
          //when mode is daily
          if (repeatOptions[index] == "daily") {
            dispatch({ type: CHANGE_TASK_REPEAT_EVERY, payload: 1 });
            dispatch({
              type: CHANGE_TASK_REPEAT_UNTIL,
              payload: new Date(defaultUntilDateDaily())
                .toISOString()
                .substring(0, 10),
            });
          }
          //when mode is weekly
          if (repeatOptions[index] == "custom")
            dispatch({
              type: CHANGE_TASK_REPEAT_WEEK_DAYS,
              payload: ["MONDAY"],
            });
          console.log(task);
          if (task.id) {
            if (task.groupId)
              dispatch(
                changeTaskRepeatMode(
                  task.id,
                  repeatModeTranslator(repeatOptions[index]),
                  task.repeatEvery,
                  task.weekDays,
                  task.repeatUntil
                    ? task.repeatUntil
                    : new Date(defaultUntilDateWeekly())
                        .toISOString()
                        .substring(0, 10),
                  task.groupId
                )
              );
            else
              dispatch(
                changeTaskRepeatMode(
                  task.id,
                  repeatModeTranslator(repeatOptions[index]),
                  task.repeatEvery,
                  task.weekDays,
                  task.repeatUntil
                    ? task.repeatUntil
                    : new Date(defaultUntilDateWeekly())
                        .toISOString()
                        .substring(0, 10)
                )
              );
          }
        }}
        hasBorder
      />
      {task.repeatMode && (
        <DatePicker
          disabled={!!task.id}
          title={"until"}
          min={new Date(task.startAt)}
          date={
            task.repeatUntil ? new Date(task.repeatUntil) : defaultUntilDate()
          }
          hasBorder
          onChange={(value: string) => {
            dispatch({ type: CHANGE_TASK_REPEAT_UNTIL, payload: value });
            if (task.id && task.groupId)
              dispatch(
                changeTaskRepeatMode(
                  task.id,
                  task.repeatMode,
                  task.repeatEvery,
                  task.weekDays,
                  value,
                  task.groupId
                )
              );
          }}
        />
      )}
      {task.repeatMode === "WEEK" && (
        <>
          <NumberPicker
            title={"every"}
            disabled={!!task.id}
            min={1}
            max={9}
            after={"week(s)"}
            value={task.repeatEvery}
            onChange={(value: string) => {
              dispatch({ type: CHANGE_TASK_REPEAT_EVERY, payload: value });
              if (task.id && task.groupId)
                dispatch(
                  changeTaskRepeatMode(
                    task.id,
                    task.repeatMode,
                    value,
                    task.repeatUntil,
                    task.weekDays,
                    task.groupId
                  )
                );
            }}
            hasBorder
          />
          <WeekPicker
            value={task.weekDays}
            disabled={!!task.id}
            onChange={(value: Array<string>) => {
              dispatch({ type: CHANGE_TASK_REPEAT_WEEK_DAYS, payload: value });
              if (task.id && task.groupId) {
                if (task.groupId)
                  dispatch(
                    changeTaskRepeatMode(
                      task.id,
                      task.repeatMode,
                      task.repeatEvery,
                      value,
                      task.repeatUntil,
                      task.groupId
                    )
                  );
              }
            }}
          />
        </>
      )}
    </div>
  );
};
