import divider from "../../images/divider.svg";

type Props = {
  label: string;
  date?: string;
  direction?: string;
};

export const ArchivedItem = ({ label, date, direction }: Props) => {
  return (
    <div className="w-full bg-white rounded-lg drop-shadow-sm cursor-pointer">
      <div className={"gap-4 flex justify-between p-2 mt-4 mb"}>
        <div className="flex  gap-2">
          <span className={"font-semibold text-sm"}>{label}</span>
        </div>
      </div>
      <div className={"gap flex justify-start pb-2 w-full ml-1"}>
        {date && <Details label={new Date(date).toLocaleString()} hasBorder />}
        {direction && <Details label={direction} textColor="text-yellow-500" />}
      </div>
    </div>
  );
};

type DetailsProps = {
  icon?: string;
  label?: string;
  hasBorder?: boolean;
  textColor?: string;
};

export const Details = ({
  icon,
  label,
  hasBorder,
  textColor,
}: DetailsProps) => {
  return (
    <div className={`flex flex-nowrap gap-1 mr-1  ${!icon ? "ml-5" : "ml-1"}`}>
      {icon && <img src={icon} alt=""></img>}
      <span
        className={
          "font-semibold inline whitespace-nowrap text-xs text-blue-500 mr-1 " +
          textColor
        }
      >
        {label}
      </span>
      {hasBorder && <img src={divider} alt=""></img>}
    </div>
  );
};
