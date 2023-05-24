import divider from "../../../images/divider.svg";

type Props = {
  title: string;
  min: number;
  max: number;
  after: string;
  hasBorder?: boolean;
  value: number;
  onChange: Function;
}
export const NumberPicker = ({
  title,
  min,
  max,
  hasBorder,
  after,
  value,
  onChange,
}: Props) => {
  return (
    <div className={"flex justify-between items-center pl-2"}>
      <p className={"font-semibold text-sm text-blue-500 pr-1 pt-0.5"}>
        {title}
      </p>
      <input
        type="number"
        className="font-semibold text-sm text-blue-500 focus:outline-none w-8"
        defaultValue={value}
        onChange={(e) => onChange(e.target.value)}
        min={min}
        max={max}
      />
      <p
        className={
          "font-semibold text-sm text-blue-500 pr-2 whitespace-nowrap  pt-0.5"
        }
      >
        {after}{" "}
      </p>
      {hasBorder && <img src={divider} alt=""></img>}
    </div>
  );
};