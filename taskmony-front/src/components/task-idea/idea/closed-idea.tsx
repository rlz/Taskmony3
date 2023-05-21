import followBlue from "../../../images/followed.svg";
import followGray from "../../../images/follow.svg";
import divider from "../../../images/divider.svg";
import commentsI from "../../../images/comment2.svg";
import createdByI from "../../../images/by.svg";
import postponeGray from "../../../images/circle-down-gray.svg";
import postponeBlue from "../../../images/circle-down-blue.svg";

type ClosedIdeaProps = {
    label: string;
    checked?: boolean;
    followed?: boolean;
    comments?: number;
    recurrent?: string;
    createdBy?: string;
    direction?: string | null;
    last: boolean;
    generation: string;
    changeFollowed: Function;
    review: Function;
  };
  
  export const ClosedIdea = ({
    label,
    followed,
    comments,
    createdBy,
    generation,
    direction,
    changeFollowed,
    review,
    last,
  }: ClosedIdeaProps) => {
    return (
      <div className="uneditedIdea w-full bg-white rounded-lg drop-shadow-sm cursor-pointer">
        <div className={"gap-4 flex justify-between p-2 mt-4 mb"}>
          <div className="flex  gap-2">
            <span className={"font-semibold text-sm"}>{label}</span>
          </div>
          <div className="relative z-30 flex gap-2">
            {typeof followed !== "undefined" && (
              <img
                alt=""
                className="w-4"
                src={followed ? followBlue : followGray}
                onClick={(e) => {
                  e.stopPropagation();
                  changeFollowed(!followed);
                }}
              ></img>
            )}
  
            <img
            alt=""
              src={last ? postponeGray : postponeBlue}
              className="w-4"
              onClick={(e) => {
                e.stopPropagation();
                if (last) return;
                review();
              }}
            ></img>
          </div>
        </div>
        <div className={"gap flex justify-start pb-2 w-full ml-1"}>
          {/* <IdeaDetails icon={createdByI} label={`${reviewedAt}`} hasBorder /> */}
          {createdBy && (
            <IdeaDetails icon={createdByI} label={`by ${createdBy}`} hasBorder />
          )}
          {
            <IdeaDetails
              icon={commentsI}
              label={comments ? comments.toString() : "0"}
              hasBorder
            />
          }
          {direction && (
            <IdeaDetails
              label={direction}
              textColor="text-yellow-500"
              hasBorder
            />
          )}
          {
            <IdeaDetails
              label={"  " + generation.toLowerCase().replaceAll("_", " ")}
              textColor="text-yellow-500"
            />
          }
        </div>
      </div>
    );
  };
  
  type IdeaDetailsProps = {
    icon?: string;
    label?: string;
    hasBorder?: boolean;
    textColor?: string;
  };
  
  export const IdeaDetails = ({
    icon,
    label,
    hasBorder,
    textColor,
  }: IdeaDetailsProps) => {
    return (
      <div className={`flex flex-nowrap gap-1 ml-1  ${icon !== undefined ? "mr-5" : "mr-1"}`}>
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
  