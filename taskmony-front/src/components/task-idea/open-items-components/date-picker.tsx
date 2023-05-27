import divider from "../../../images/divider.svg";


type Props = {
  title: string;
  date: Date;
  hasBorder?: boolean;
  onChange: Function;
  min?: Date;
};
export const DatePicker = ({
  title,
  date,
  onChange,
  hasBorder,
  min,
}: Props) => {
  return (
    <div className={"flex justify-between items-center pl-2"}>
      <p className={"font-semibold text-sm text-blue-500 whitespace-nowrap"}>
        {title}:
      </p>
      <input
        type="date"
        className="font-semibold border-none text-sm text-blue-500 focus:outline-none w-28 pr-1"
        value={new Date(date).toISOString().substring(0, 10)}
        min={
          min
            ? new Date(min.setDate(min.getDate() + 1))
                .toISOString()
                .substring(0, 10)
            : new Date().toISOString().substring(0, 10)
        }
        onChange={(e) => onChange(new Date(e.target.value).toISOString())}
      />
      {hasBorder && <img src={divider} alt=""></img>}
    </div>
  );
};
