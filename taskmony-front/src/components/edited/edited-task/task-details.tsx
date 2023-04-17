import { changeTaskAssignee, changeTaskDirection, changeTaskRepeatMode, changeTaskRepeatUntil, changeTaskStartAt, CHANGE_TASK_ASSIGNEE, CHANGE_TASK_DIRECTION, CHANGE_TASK_REPEAT_EVERY, CHANGE_TASK_REPEAT_MODE, CHANGE_TASK_REPEAT_UNTIL, CHANGE_TASK_REPEAT_WEEK_DAYS, CHANGE_TASK_START_DATE } from "../../../services/actions/tasksAPI";
import { useAppDispatch, useAppSelector } from "../../../utils/hooks";
import { DatePicker } from "../date-picker";
import { ItemPicker } from "../item-picker";
import { NumberPicker } from "../number-picker";
import { WeekPicker } from "../week-picker";

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
  const defaultUntilDate = () => {
    const date = new Date(Date.now());
    return new Date(date.setFullYear(date.getFullYear() + 1));
  };
  const members = directions.filter((d) => d.id == task.direction?.id)[0]
    ?.members;

  return (
    <div className={"gap flex justify-start pb-2 w-full ml-1"}>
      {!fromDirection && (
        <ItemPicker
          title={"direction"}
          option={task.direction?.name ? task.direction?.name : "none"}
          options={["none", ...directions.map((dir) => dir.name)]}
          onChange={(index: number) => {
            const payload = index == 0 ? null : directions[index - 1];
            dispatch({ type: CHANGE_TASK_DIRECTION, payload: payload });
            if (task.id && payload)
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
            if (task.id) dispatch(changeTaskAssignee(task.id, payload));
          }}
          hasBorder
          width="w-24"
        />
      )}
      <DatePicker
        title={"start date"}
        date={task.startAt ? new Date(task.startAt) : new Date()}
        hasBorder
        onChange={(value: string) => {
          dispatch({ type: CHANGE_TASK_START_DATE, payload: value });
          if (task.id) dispatch(changeTaskStartAt(task.id, value));
        }}
      />
      <ItemPicker
        title={"repeated"}
        options={repeatOptions}
        option={repeatModeTranslator(task.repeatMode) || "no"}
        onChange={(index: number) => {
          dispatch({
            type: CHANGE_TASK_REPEAT_MODE,
            payload: repeatModeTranslator(repeatOptions[index]),
          });
          //when mode is weekly or daily
          if (
            repeatOptions[index] == "daily" ||
            repeatOptions[index] == "custom"
          ) {
            dispatch({ type: CHANGE_TASK_REPEAT_EVERY, payload: 1 });
            dispatch({
              type: CHANGE_TASK_REPEAT_UNTIL,
              payload: new Date(defaultUntilDate())
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
          if (task.id)
            dispatch(
              changeTaskRepeatMode(
                task.id,
                repeatModeTranslator(repeatOptions[index]),
                task.repeatEvery,
                task.weekDays
              )
            );
        }}
        hasBorder
      />
      {task.repeatMode && (
        <DatePicker
          title={"until"}
          min={new Date(task.startAt)}
          date={
            task.repeatUntil ? new Date(task.repeatUntil) : defaultUntilDate()
          }
          hasBorder
          onChange={(value: string) => {
            dispatch({ type: CHANGE_TASK_REPEAT_UNTIL, payload: value });
            if (task.id) dispatch(changeTaskRepeatUntil(task.id, value));
          }}
        />
      )}
      {task.repeatMode === "WEEK" && (
        <>
          <NumberPicker
            title={"every"}
            min={1}
            max={9}
            after={"week(s)"}
            value={task.repeatEvery}
            onChange={(value: string) => {
              dispatch({ type: CHANGE_TASK_REPEAT_EVERY, payload: value });
              if (task.id)
                dispatch(
                  changeTaskRepeatMode(
                    task.id,
                    task.repeatMode,
                    value,
                    task.weekDays
                  )
                );
            }}
            hasBorder
          />
          <WeekPicker
            value={task.weekDays}
            onChange={(value: Array<string>) => {
              dispatch({ type: CHANGE_TASK_REPEAT_WEEK_DAYS, payload: value });
              if (task.id)
                dispatch(
                  changeTaskRepeatMode(
                    task.id,
                    task.repeatMode,
                    task.repeatEvery,
                    value
                  )
                );
            }}
          />
        </>
      )}
    </div>
  );
};