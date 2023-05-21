import { AddBtn } from "../btn";
import { Comment, CommentInput } from "./comment";
import { useState } from "react";
import add from "../../../../images/add-light.svg";
import { TComment } from "../../../../utils/types";

type CommentsInputProps = {
  comments: Array<TComment>;
  send: Function;
};

export const Comments = ({comments,send} : CommentsInputProps) => {
  const [showInput, setShowInput] = useState<boolean>(false);
  const [commentInput, setCommentInput] = useState<string>("");
  const sendComment = () => {
    send(commentInput);
    setShowInput(false);
    setCommentInput("");
  };

  return (
    <>
      {comments?.map((comment : TComment, index: number) => (
        <Comment
          key={index}
          text={comment.text}
          author={comment.createdBy.displayName}
          time={comment.createdAt} />
      ))}
      {showInput && (
        <CommentInput
          commentValue={commentInput}
          changeComment={setCommentInput}
          send={sendComment} />
      )}
      <div className="flex justify-center p-1">
        <AddBtn
          label={commentInput ? "send comment" : "add a new comment"}
          icon={commentInput ? undefined : add}
          onClick={showInput ? sendComment : () => setShowInput(true)} />
      </div>
    </>
  );
};
