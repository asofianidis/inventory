"use client";

import { Organizations } from "@/types/organizations";
import {
  Sheet,
  SheetContent,
  SheetDescription,
  SheetHeader,
  SheetTitle,
  SheetTrigger,
} from "@/components/ui/sheet";
import { Button } from "@/components/ui/button";
import { useState } from "react";
import { Label } from "@/components/ui/label";
import { Input } from "@/components/ui/input";

type props = {
  organization?: Organizations;
};

export default function ModifyOrganization({ organization }: props) {
  const [name, setName] = useState(organization?.name ?? "");
  const handleUpdate = async () => {
    try {
    } catch (e: any) {
      console.log(e);
    }
  };
  return (
    <Sheet>
      <SheetTrigger asChild>
        <Button>{organization != undefined ? "Edit" : "New"}</Button>
      </SheetTrigger>
      <SheetContent>
        <SheetHeader>
          <SheetTitle>
            {organization != undefined
              ? `Edit Organization: ${organization.name}`
              : "Create New Organization"}
          </SheetTitle>
        </SheetHeader>
        <div className="flex flex-1 flex-col gap-5">
          <div className="flex flex-col gap-2">
            <Label>Organization Name</Label>
            <Input
              value={name}
              onChange={(e) => {
                setName(e.target.value);
              }}
              placeholder="Organization Name"
            />
          </div>

          <div className="flex gap-2 items-center justify-end">
            <Button>Save</Button>
            {organization != undefined ? (
              <>
                <Button variant={"destructive"}>Delete</Button>
              </>
            ) : null}
          </div>
        </div>
      </SheetContent>
    </Sheet>
  );
}
